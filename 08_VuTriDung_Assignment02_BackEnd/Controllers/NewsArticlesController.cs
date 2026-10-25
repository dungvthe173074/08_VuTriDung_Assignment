using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;
using _08_VuTriDung_Assignment02.Repositories;

namespace _08_VuTriDung_Assignment02.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticlesController(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        private static NewsArticleDTO MapToDTO(NewsArticle n)
        {
            return new NewsArticleDTO
            {
                NewsArticleID = n.NewsArticleID,
                NewsTitle = n.NewsTitle,
                Headline = n.Headline,
                CreatedDate = n.CreatedDate,
                NewsContent = n.NewsContent,
                NewsSource = n.NewsSource,
                CategoryID = n.CategoryID,
                CategoryName = n.Category?.CategoryName,
                NewsStatus = n.NewsStatus,
                CreatedByID = n.CreatedByID,
                CreatedByName = n.CreatedBy?.AccountName,
                UpdatedByID = n.UpdatedByID,
                ModifiedDate = n.ModifiedDate,
                TagIDs = n.NewsTags.Select(nt => nt.TagID).ToList(),
                Tags = n.NewsTags.Where(nt => nt.Tag != null).Select(nt => new TagDTO
                {
                    TagID = nt.TagID,
                    TagName = nt.Tag?.TagName,
                    Note = nt.Tag?.Note
                }).ToList()
            };
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        [EnableQuery]
        public IActionResult Get()
        {
            var articles = _newsArticleRepository.GetAll().Select(MapToDTO).AsQueryable();
            return Ok(articles);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult GetActive()
        {
            var activeArticles = _newsArticleRepository.GetActive().Select(MapToDTO).AsQueryable();
            return Ok(activeArticles);
        }

        [HttpGet("my-history")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetMyHistoryFromClaims()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!short.TryParse(idClaim, out var accountId) || accountId <= 0)
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var articles = _newsArticleRepository.GetByCreatedBy(accountId).Select(MapToDTO).ToList();
            return Ok(articles);
        }

        [HttpGet("my-history/{accountId}")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetMyHistory([FromRoute] short accountId)
        {
            var articles = _newsArticleRepository.GetByCreatedBy(accountId).Select(MapToDTO).ToList();
            return Ok(articles);
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var articles = _newsArticleRepository.GetReport(startDate, endDate).Select(MapToDTO).ToList();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById([FromRoute] string id)
        {
            var article = _newsArticleRepository.GetById(id);
            if (article == null)
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }
            return Ok(MapToDTO(article));
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] NewsArticleCreateUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = _newsArticleRepository.GetById(dto.NewsArticleID);
            if (existing != null)
            {
                return BadRequest(new { message = $"News article with ID {dto.NewsArticleID} already exists." });
            }

            short? createdById = dto.CreatedByID;
            if (!createdById.HasValue || createdById.Value <= 0)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (short.TryParse(idClaim, out var claimId))
                {
                    createdById = claimId;
                }
            }

            var article = new NewsArticle
            {
                NewsArticleID = dto.NewsArticleID,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate ?? DateTime.Now,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryID = dto.CategoryID,
                NewsStatus = dto.NewsStatus ?? true,
                CreatedByID = createdById,
                UpdatedByID = dto.UpdatedByID,
                ModifiedDate = DateTime.Now
            };

            var created = _newsArticleRepository.Create(article, dto.TagIDs);
            return CreatedAtAction(nameof(GetById), new { id = created.NewsArticleID }, MapToDTO(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put([FromRoute] string id, [FromBody] NewsArticleCreateUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = _newsArticleRepository.GetById(id);
            if (existing == null)
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }

            short? updatedById = dto.UpdatedByID;
            if (!updatedById.HasValue || updatedById.Value <= 0)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (short.TryParse(idClaim, out var claimId))
                {
                    updatedById = claimId;
                }
            }

            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryID = dto.CategoryID;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedByID = updatedById;

            var updated = _newsArticleRepository.Update(existing, dto.TagIDs);
            if (updated == null)
            {
                return StatusCode(500, new { message = "Error updating article." });
            }

            return Ok(MapToDTO(updated));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete([FromRoute] string id)
        {
            var deleted = _newsArticleRepository.Delete(id);
            if (!deleted)
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }

            return Ok(new { message = "News article deleted successfully." });
        }
    }
}

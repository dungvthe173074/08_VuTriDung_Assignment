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
        private readonly ICategoryRepository _categoryRepository;

        public NewsArticlesController(INewsArticleRepository newsArticleRepository, ICategoryRepository categoryRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult Get()
        {
            var articles = _newsArticleRepository.GetAll();
            return Ok(articles);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult GetActive()
        {
            var activeArticles = _newsArticleRepository.GetActive();
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

            var articles = _newsArticleRepository.GetByCreatedBy(accountId);
            return Ok(articles);
        }

        [HttpGet("my-history/{accountId}")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetMyHistory([FromRoute] short accountId)
        {
            var articles = _newsArticleRepository.GetByCreatedBy(accountId);
            return Ok(articles);
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var articles = _newsArticleRepository.GetReport(startDate, endDate);
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

            return Ok(article);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] NewsArticleCreateUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrWhiteSpace(dto.NewsArticleID) && _newsArticleRepository.Exists(dto.NewsArticleID))
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

            // Category validation
            if (dto.CategoryID.HasValue)
            {
                var category = _categoryRepository.GetById(dto.CategoryID.Value);
                if (category == null || category.IsActive != true)
                {
                    return BadRequest(new { message = "Category must exist and be active to assign to an article." });
                }
            }

            var articleId = string.IsNullOrWhiteSpace(dto.NewsArticleID)
                ? Guid.NewGuid().ToString("N")[..10]
                : dto.NewsArticleID;

            var article = new NewsArticle
            {
                NewsArticleID = articleId,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate ?? DateTime.Now,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryID = dto.CategoryID,
                NewsStatus = dto.NewsStatus ?? true,
                CreatedByID = createdById,
                UpdatedByID = createdById,
                ModifiedDate = DateTime.Now
            };

            var created = _newsArticleRepository.Create(article, dto.TagIDs);
            return CreatedAtAction(nameof(GetById), new { id = created.NewsArticleID }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put([FromRoute] string id, [FromBody] NewsArticleCreateUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_newsArticleRepository.Exists(id))
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }

            short? updatedById = dto.UpdatedByID;
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (short.TryParse(idClaim, out var claimId))
            {
                updatedById = claimId;
            }

            // Category validation
            if (dto.CategoryID.HasValue)
            {
                var category = _categoryRepository.GetById(dto.CategoryID.Value);
                if (category == null || category.IsActive != true)
                {
                    return BadRequest(new { message = "Category must exist and be active to assign to an article." });
                }
            }

            var updated = _newsArticleRepository.Update(id, dto, updatedById);
            if (updated == null)
            {
                return StatusCode(500, new { message = "Error updating article." });
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete([FromRoute] string id)
        {
            if (!_newsArticleRepository.Exists(id))
            {
                return NotFound(new { message = $"News article with ID {id} not found." });
            }

            var deleted = _newsArticleRepository.Delete(id);
            if (!deleted)
            {
                return StatusCode(500, new { message = "Error deleting article." });
            }

            return Ok(new { message = "News article deleted successfully." });
        }
    }
}

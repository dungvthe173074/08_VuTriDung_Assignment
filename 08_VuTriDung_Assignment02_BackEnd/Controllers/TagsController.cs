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
    public class TagsController : ODataController
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        [EnableQuery]
        public IActionResult Get()
        {
            var tags = _tagRepository.GetAll().AsQueryable();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Admin")]
        [EnableQuery]
        public IActionResult Get([FromRoute] int id)
        {
            var tag = _tagRepository.GetById(id);
            if (tag == null)
            {
                return NotFound(new { message = $"Tag with ID {id} not found." });
            }
            return Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] TagDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = new Tag
            {
                TagID = dto.TagID,
                TagName = dto.TagName,
                Note = dto.Note
            };

            var created = _tagRepository.Create(tag);
            return CreatedAtAction(nameof(Get), new { id = created.TagID }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put([FromRoute] int id, [FromBody] TagDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = _tagRepository.GetById(id);
            if (existing == null)
            {
                return NotFound(new { message = $"Tag with ID {id} not found." });
            }

            existing.TagName = dto.TagName;
            existing.Note = dto.Note;

            var updated = _tagRepository.Update(existing);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete([FromRoute] int id)
        {
            var deleted = _tagRepository.Delete(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Tag with ID {id} not found." });
            }

            return Ok(new { message = "Tag deleted successfully." });
        }
    }
}

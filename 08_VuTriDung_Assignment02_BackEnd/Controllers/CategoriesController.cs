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
    public class CategoriesController : ODataController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult Get()
        {
            var categories = _categoryRepository.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult Get([FromRoute] short id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryID = dto.ParentCategoryID,
                IsActive = dto.IsActive ?? true
            };

            var created = _categoryRepository.Create(category);
            return CreatedAtAction(nameof(Get), new { id = created.CategoryID }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put([FromRoute] short id, [FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                CategoryID = id,
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryID = dto.ParentCategoryID,
                IsActive = dto.IsActive
            };

            var updated = _categoryRepository.Update(id, category);
            if (updated == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete([FromRoute] short id)
        {
            var result = _categoryRepository.Delete(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}

using AutoMapper;
using BusinessObject;
using DTO.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    [Route("odata/Categories")]
    [ApiController]
    public class CategoriesController : ODataController
    {
        public ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: api/<CategoryController>
        [EnableQuery]
        [HttpGet("")]
        public ActionResult<IEnumerable<Category>> GetAll()
        {
            var categories = _categoryService.GetAllCategory();
            //var responseCategoty = _mapper.Map<IEnumerable<ResponseCategoryDTO>>(categories);
            return Ok(categories);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{key}")]
        [Authorize(Roles = "1")]
        public ActionResult<Category> GetById([FromODataUri] short key)
        {
            var category = _categoryService.GetCategoryById(key);
            if (category == null) { return NotFound(); }
            //var responseCategoty = _mapper.Map<ResponseCategoryDTO>(category);
            return Ok(category);
        }

        // POST api/<CategoryController>
        [HttpPost("")]
        [Authorize(Roles = "1")]
        public IActionResult Post([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = _mapper.Map<Category>(createCategoryDTO);
            _categoryService.CreateCategory(category);
            return Created(category);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{key}")]
        [Authorize(Roles = "1")]
        public ActionResult Put([FromODataUri] short key, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = _categoryService.GetCategoryById(key);
            if (exist == null)
            {
                return NotFound();
            }
            var category = _mapper.Map<Category>(updateCategoryDTO);
            category.CategoryId = exist.CategoryId;
            category.IsActive = true;
            _categoryService.UpdateCategory(category);
            return Created(category);

        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{key}")]
        [Authorize(Roles = "1")]
        public IActionResult Delete([FromODataUri] short key)
        {
            var category = _categoryService.GetCategoryById(key);
            if (category == null)
            {
                return NotFound();
            }
            int categoryStatus = _categoryService.DeleteCategory(key);
            if(categoryStatus == 0)
            {
                return StatusCode(400, "Danh mục còn ở trong tin tức");
            }else if(categoryStatus == 1)
            {
                return Ok("Xóa thành công");
            }
            return NotFound(); 
        }
    }
}

using AutoMapper;
using BusinessObject;
using DTO.Category;
using DTO.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    [Route("odata/Tags")]
    [ApiController]
    public class TagsController : ODataController
    {
        public ITagService _tagService;
        private readonly IMapper _mapper;
        public TagsController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        // GET: api/<CategoryController>
        [EnableQuery]
        [HttpGet("")]
        [Authorize(Roles = "1")]
        public ActionResult<IEnumerable<Tag>> GetAll()
        {
            var tags = _tagService.GetAllTag();
            //var responseCategoty = _mapper.Map<IEnumerable<ResponseCategoryDTO>>(categories);
            return Ok(tags);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{key}")]
        [Authorize(Roles = "1")]
        public ActionResult<Tag> GetById([FromODataUri] short key)
        {
            var tag = _tagService.GetTagById(key);
            if (tag == null) { return NotFound(); }
            //var responseCategoty = _mapper.Map<ResponseCategoryDTO>(category);
            return Ok(tag);
        }

        // POST api/<CategoryController>
        [HttpPost("")]
        [Authorize(Roles = "1")]
        public IActionResult Post([FromBody] CreateTagDTO createTagDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tag = _mapper.Map<Tag>(createTagDTO);
            _tagService.CreateTag(tag);
            return Created(tag);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{key}")]
        [Authorize(Roles = "1")]
        public ActionResult Put([FromODataUri] short key, [FromBody] UpdateTagDTO updateTagDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = _tagService.GetTagById(key);
            if (exist == null)
            {
                return NotFound();
            }
            var tag = _mapper.Map<Tag>(updateTagDTO);
            tag.TagId = exist.TagId;
            _tagService.UpdateTag(tag);
            return Created(tag);

        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{key}")]
        [Authorize(Roles = "1")]
        public IActionResult Delete([FromODataUri] short key)
        {
            var tag = _tagService.GetTagById(key);
            if (tag == null)
            {
                return NotFound();
            }
            _tagService.DeleteTag(key);
            return Ok("Xóa thành công");
            
        }
    }
}

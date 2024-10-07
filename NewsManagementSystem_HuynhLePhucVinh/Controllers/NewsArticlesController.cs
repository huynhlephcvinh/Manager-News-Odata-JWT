using AutoMapper;
using BusinessObject;
using DTO.News;
using DTO.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    [Route("odata/NewsArticles")]
    [ApiController]
    public class NewsArticlesController : ODataController
    {
        public INewsArticleService _newsArticleService;
        public ISystemAccountService _systemAccountService;
        private readonly IMapper _mapper;
        
        public NewsArticlesController(INewsArticleService newsArticleService, 
            IMapper mapper, 
            ISystemAccountService systemAccountService
        )
        {
            _newsArticleService = newsArticleService;
            _mapper = mapper;
            _systemAccountService = systemAccountService;
        }

        // GET: api/<CategoryController>
        [EnableQuery]
        [HttpGet("")]
        public ActionResult<IEnumerable<NewsArticle>> GetAll()
        {
            var news = _newsArticleService.GetAllNews();
            //var responseCategoty = _mapper.Map<IEnumerable<ResponseCategoryDTO>>(categories);
            return Ok(news);
        }

        [EnableQuery]
        [HttpGet("history-created")]
        [Authorize(Roles = "1")]
        public ActionResult<IEnumerable<NewsArticle>> GetAllHistoryCreated()
        {
            string? userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var news = _newsArticleService.GetAllNews().Where(x=>x.CreatedById == short.Parse(userId));
            //var responseCategoty = _mapper.Map<IEnumerable<ResponseCategoryDTO>>(categories);
            return Ok(news);
        }

        [EnableQuery]
        [HttpGet("statistics")]
        [Authorize(Roles = "3")]
        public ActionResult<IEnumerable<NewsArticle>> GetAllStatistics(DateTime startDate, DateTime endDate)
        {

            var news = _newsArticleService.LoadNewsArticlesReportStatistics(startDate, endDate);

            return Ok(news);
        }
        // GET api/<CategoryController>/5
        [HttpGet("{key}")]
        [Authorize(Roles = "1")]
        [EnableQuery]
        public ActionResult<NewsArticle> GetById([FromODataUri] string key)
        {
            var news = _newsArticleService.GetNewsById(key);
            if (news == null) { return NotFound(); }
            //var responseCategoty = _mapper.Map<ResponseCategoryDTO>(category);
            return Ok(news);
        }

        // POST api/<CategoryController>
        [HttpPost("")]
        [Authorize(Roles = "1")]
        public IActionResult Post([FromBody] CreateNewsDTO createNewsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var news = _mapper.Map<NewsArticle>(createNewsDTO);
      //      var authenHeader = _httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();
            string? userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _systemAccountService.GetSystemAccount(short.Parse(userId));
            _newsArticleService.CreateNews(news, user, createNewsDTO.idTags);
            return Created(news);
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{key}")]
        [Authorize(Roles = "1")]
        public ActionResult Put([FromODataUri] string key, [FromBody] UpdateNewsDTO updateNewsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = _newsArticleService.GetNewsById(key);
            if (exist == null)
            {
                return NotFound();
            }
            var news = _mapper.Map<NewsArticle>(updateNewsDTO);
            news.NewsArticleId = exist.NewsArticleId;
            string? userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _systemAccountService.GetSystemAccount(short.Parse(userId));
            _newsArticleService.UpdateNews(news, user);
            return Created(news);

        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{key}")]
        [Authorize(Roles = "1")]
        public IActionResult Delete([FromODataUri] string key)
        {
            var news = _newsArticleService.GetNewsById(key);
            if (news == null)
            {
                return NotFound();
            }
            _newsArticleService.DeleteNews(key);
            return Ok("Xóa thành công");

        }

        [HttpPost("add-tags")]
        [Authorize(Roles = "1")]
        public IActionResult AddTagToNews([FromBody] TagToNewsDTO addTagToNewsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _newsArticleService.AddTagToNews(addTagToNewsDTO.NewsArticleId, addTagToNewsDTO.TagId);
            return Ok("Thêm thành công");
        }

        [HttpPost("remove-tags")]
        [Authorize(Roles = "1")]
        public IActionResult DeleteTagToNews([FromBody] TagToNewsDTO deleteTagToNewsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _newsArticleService.RemoveTag(deleteTagToNewsDTO.NewsArticleId, deleteTagToNewsDTO.TagId);
            return Ok("Xóa thành công");

        }
    }
}

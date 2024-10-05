using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using UniversityMVC.Models;
using Microsoft.AspNetCore.Http;

namespace UniversityMVC.Controllers
{
    public class BaseController : Controller
    {
        protected readonly HttpClient _httpClient;
        protected readonly string CategoryAPIURL;
        protected readonly string NewsArticleAPIURL;
        protected readonly string SystemAccountAPIURL;
        protected readonly string TagAPIURL;
        protected readonly string AuthAPIURL;
        public BaseController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);

            var session = httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var token = session.GetString("JWTToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            var apiSettings = new ApiSettings();
            configuration.GetSection("ApiUrls").Bind(apiSettings);
            CategoryAPIURL = apiSettings.CategoryAPIURL;
            NewsArticleAPIURL = apiSettings.NewsArticleAPIURL;
            SystemAccountAPIURL = apiSettings.SystemAccountAPIURL;
            TagAPIURL = apiSettings.TagAPIURL;
            AuthAPIURL = apiSettings.AuthAPIURL;
        }
    }
}

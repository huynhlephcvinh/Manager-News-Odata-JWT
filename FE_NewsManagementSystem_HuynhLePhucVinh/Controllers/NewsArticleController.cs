using Azure;
using BusinessObject;
using DTO.Category;
using DTO.News;
using DTO.Tag;
using FE_NewsManagementSystem_HuynhLePhucVinh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using UniversityMVC.Controllers;
using static FE_NewsManagementSystem_HuynhLePhucVinh.Controllers.CategoryController;
using static FE_NewsManagementSystem_HuynhLePhucVinh.Controllers.TagController;

namespace FE_NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    public class NewsArticleController : BaseController
    {
        public NewsArticleController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
: base(httpClientFactory, configuration, httpContextAccessor) { }
        public class NewsArticleResponse
        {
            [JsonProperty("value")]
            public List<NewsArticle> Value { get; set; }

            [JsonProperty("@odata.count")]
            public int TotalCount { get; set; }
        }

        //private async Task LoadAsync()
        //{
        //    HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
        //    if (!Res.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Unable to load category.");
        //    }
        //    string categoryData = await Res.Content.ReadAsStringAsync();
        //    var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
        //    (categoryData);
        //    ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
        //}


        public async Task<IActionResult> Index(int? skip = 0, int? top = 2, string? searchString = null, int? categoryId = null)
        {
            var urlBuilder = new StringBuilder(NewsArticleAPIURL);

            urlBuilder.Append($"?$skip={skip ?? 0}&$top={top ?? 2}&$count=true&$expand=Category,CreatedBy,Tags");

            // Xây dựng bộ lọc nếu có điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(searchString) || categoryId.HasValue)
            {
                var filterConditions = new List<string>();

                if (categoryId.HasValue)
                    filterConditions.Add($"CategoryId eq {categoryId.Value}");

                if (!string.IsNullOrEmpty(searchString))
                    filterConditions.Add($"contains(tolower(NewsTitle), '{searchString.ToLower()}')");

                urlBuilder.Append($"&$filter={string.Join(" and ", filterConditions)}");
            }

            HttpResponseMessage newsResponse = await _httpClient.GetAsync(urlBuilder.ToString());

            if (!newsResponse.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            string newsData = await newsResponse.Content.ReadAsStringAsync();
            var newsArticles = JsonConvert.DeserializeObject<NewsArticleResponse>(newsData);

            HttpResponseMessage categoryResponse = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }

            string categoryData = await categoryResponse.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<CategoryResponse>(categoryData);
            ViewBag.CategoryAll = new SelectList(categories.Value, "CategoryId", "CategoryName");

            // Lưu lại các tham số tìm kiếm và phân trang để dùng lại khi chuyển trang
            ViewBag.SearchString = searchString ?? string.Empty;
            ViewBag.CategoryId = categoryId;
            ViewBag.TotalPages = (int)Math.Ceiling((double)newsArticles.TotalCount / top.Value);
            ViewBag.CurrentPage = skip.Value / top.Value + 1;
            ViewBag.PageSize = top;

            return View(newsArticles.Value);
        }



        public async Task<IActionResult> ReportStatistic(int? skip = 0, int? top = 2, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Base API URL
            string str = NewsArticleAPIURL + "?$expand=Category,CreatedBy,Tags";

            // Pagination
            if (skip != null && top != null)
            {
                str += "&$skip=" + skip + "&$top=" + top + "&$count=true";
            }

            // Filter by date range (if specified)
            List<string> filters = new List<string>();
            if (startDate.HasValue)
            {
                filters.Add($"CreatedDate ge {startDate.Value:yyyy-MM-dd}");
            }
            if (endDate.HasValue)
            {
                filters.Add($"CreatedDate le {endDate.Value:yyyy-MM-dd}");
            }
            if (filters.Any())
            {
                str += "&$filter=" + string.Join(" and ", filters);
            }

            // Order by CreatedDate descending
            str += "&$orderby=CreatedDate desc";

            // Call the API
            HttpResponseMessage res = await _httpClient.GetAsync(str);

            // Handle response error
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            // Deserialize response
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<NewsArticleResponse>(rData);

            // Pagination values
            int totalRecords = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / top.Value);
            ViewBag.CurrentPage = skip.Value / top.Value + 1;
            ViewBag.PageSize = top;

            // Pass the current startDate and endDate to the view for re-population of the fields
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Return data to view
            return View(response.Value);
        }



        public async Task<IActionResult> ListHistoryNews(int? skip = 0, int? top = 2)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            string str = "";
            str = NewsArticleAPIURL;
            if (skip != null && top != null)
            {
                str += "/history-created";
            }
            HttpResponseMessage res = await _httpClient.GetAsync(str);
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<NewsArticleResponse>(rData);

            int totalRecords = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / top.Value);
            ViewBag.CurrentPage = skip.Value / top.Value + 1;
            ViewBag.PageSize = top;
            if (response.Value.Any())
            {
                return View(response.Value);
            }
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier,
                ContentError = "Chưa có news nào tạo"
            });


        }

        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
            if (!Res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await Res.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);
            ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsDTO newsDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(newsDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync(NewsArticleAPIURL, content);
                //string responseContent = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Server Error");
            }

            HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
            if (!Res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await Res.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);
            ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
            return View(newsDTO);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            string str = "";
            str = NewsArticleAPIURL;
            HttpResponseMessage res = await _httpClient.GetAsync($"{str}/{id}?$expand=Category,CreatedBy,Tags");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<NewsArticle>(rData);

            HttpResponseMessage categoryRes = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await categoryRes.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);

            HttpResponseMessage tagRes = await _httpClient.GetAsync(TagAPIURL);
            if (!tagRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load tag.");
            }
            string tagData = await tagRes.Content.ReadAsStringAsync();
            var tagResponse = JsonConvert.DeserializeObject<TagResponse>
            (tagData);
            ViewBag.GetAllTag = new SelectList(tagResponse.Value, "TagId", "TagName");
            ViewData["CategorySelected"] = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName", response.CategoryId);
            ViewBag.Tags = response.Tags;
     
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateNewsDTO updateNewsDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(updateNewsDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PutAsync($"{NewsArticleAPIURL}/{id}", content);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Server Error");
            }


            HttpResponseMessage tagRes = await _httpClient.GetAsync(TagAPIURL);
            if (!tagRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load tag.");
            }
            string tagData = await tagRes.Content.ReadAsStringAsync();
            var tagResponse = JsonConvert.DeserializeObject<TagResponse>
            (tagData);
            ViewBag.GetAllTag = new SelectList(tagResponse.Value, "TagId", "TagName");
            return View(updateNewsDTO);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTagNews(TagToNewsDTO removeTagNewsDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(removeTagNewsDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync($"{NewsArticleAPIURL}/remove-tags", content);

                if (res.IsSuccessStatusCode)
                {
                    // Successfully removed the tag, redirect back to the Edit page with the article ID
                    return RedirectToAction("Edit", new { id = removeTagNewsDTO.NewsArticleId });
                }

                ModelState.AddModelError(string.Empty, "Server Error");
            }

            // If ModelState is invalid or there was an error, reload the category and tag lists
            HttpResponseMessage categoryRes = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }

            string categoryData = await categoryRes.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>(categoryData);
            ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
            HttpResponseMessage tagRes = await _httpClient.GetAsync(TagAPIURL);
            if (!tagRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load tag.");
            }
            string tagData = await tagRes.Content.ReadAsStringAsync();
            var tagResponse = JsonConvert.DeserializeObject<TagResponse>
            (tagData);
            ViewBag.GetAllTag = new SelectList(tagResponse.Value, "TagId", "TagName");

            // Reload tags for the current article
            HttpResponseMessage articleRes = await _httpClient.GetAsync($"{NewsArticleAPIURL}/{removeTagNewsDTO.NewsArticleId}");
            if (!articleRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load the article.");
            }

            string articleData = await articleRes.Content.ReadAsStringAsync();
            var articleResponse = JsonConvert.DeserializeObject<NewsArticle>(articleData);
            ViewBag.Tags = articleResponse.Tags;

            return RedirectToAction("Edit", new { id = removeTagNewsDTO.NewsArticleId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTagNews(TagToNewsDTO addTagNewsDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(addTagNewsDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync($"{NewsArticleAPIURL}/add-tags", content);

                if (res.IsSuccessStatusCode)
                {
                    // Successfully added the tag, redirect back to the Edit page with the article ID
                    return RedirectToAction("Edit", new { id = addTagNewsDTO.NewsArticleId });
                }

                ModelState.AddModelError(string.Empty, "Server Error");
            }

            // If ModelState is invalid or there was an error, reload the category and tag lists
            HttpResponseMessage categoryRes = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }

            string categoryData = await categoryRes.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>(categoryData);
            ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");

            HttpResponseMessage tagRes = await _httpClient.GetAsync(TagAPIURL);
            if (!tagRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load tag.");
            }
            string tagData = await tagRes.Content.ReadAsStringAsync();
            var tagResponse = JsonConvert.DeserializeObject<TagResponse>(tagData);
            ViewBag.GetAllTag = new SelectList(tagResponse.Value, "TagId", "TagName");

            // Reload tags for the current article
            HttpResponseMessage articleRes = await _httpClient.GetAsync($"{NewsArticleAPIURL}/{addTagNewsDTO.NewsArticleId}");
            if (!articleRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load the article.");
            }

            string articleData = await articleRes.Content.ReadAsStringAsync();
            var articleResponse = JsonConvert.DeserializeObject<NewsArticle>(articleData);
            ViewBag.Tags = articleResponse.Tags;

            return RedirectToAction("Edit", new { id = addTagNewsDTO.NewsArticleId });
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.DeleteAsync($"{NewsArticleAPIURL}/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error");
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.GetAsync($"{NewsArticleAPIURL}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<NewsArticle>(rData);
            HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
            if (!Res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await Res.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);
            ViewBag.CategoryAll = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
            return View(response);

        }
    }
}

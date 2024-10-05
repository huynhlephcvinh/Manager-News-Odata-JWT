using BusinessObject;
using DTO.Category;
using FE_NewsManagementSystem_HuynhLePhucVinh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using UniversityMVC.Controllers;
using static FE_NewsManagementSystem_HuynhLePhucVinh.Controllers.NewsArticleController;

namespace FE_NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
       : base(httpClientFactory, configuration, httpContextAccessor) { }


        public class CategoryResponse
        {
            [JsonProperty("value")]
            public List<Category> Value { get; set; }
            [JsonProperty("@odata.count")]
            public int TotalCount { get; set; }
        }

        //private async Task LoadParentCategoriesAsync()
        //{
        //    HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
        //    if (!Res.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Unable to load specializations.");
        //    }
        //    string parentCategoryData = await Res.Content.ReadAsStringAsync();
        //    var parentCategoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
        //    (parentCategoryData);
        //    ViewBag.ParentCategory = new SelectList(parentCategoryResponse.Value, "CategoryId", "CategoryName");
        //}
        public async Task<IActionResult> Index(int? skip = 0, int? top = 2, string? searchString = null)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }

            string str = CategoryAPIURL;

            if (skip != null && top != null)
            {
                str += "?$skip=" + skip + "&$top=" + top + "&$count=true&$expand=ParentCategory";
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                str += $"&$filter=contains(tolower(CategoryName), '{searchString.ToLower()}')";
            }

            HttpResponseMessage res = await _httpClient.GetAsync(str);

            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CategoryResponse>(rData);

            int totalRecords = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / top.Value);
            ViewBag.CurrentPage = skip.Value / top.Value + 1;
            ViewBag.PageSize = top;
            ViewBag.SearchString = searchString ?? string.Empty;

            return View(response.Value);
        }


        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage categoryRes = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await categoryRes.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);
            ViewBag.ParentCategory = new SelectList(categoryResponse.Value, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDTO categoryDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(categoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync(CategoryAPIURL, content);
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
                throw new Exception("Unable to load specializations.");
            }
            string parentCategoryData = await Res.Content.ReadAsStringAsync();
            var parentCategoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (parentCategoryData);
            ViewBag.ParentCategory = new SelectList(parentCategoryResponse.Value, "CategoryId", "CategoryName");
            return View(categoryDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            string str = "";
            str = CategoryAPIURL;
            HttpResponseMessage res = await _httpClient.GetAsync($"{str}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Category>(rData);

            HttpResponseMessage categoryRes = await _httpClient.GetAsync(CategoryAPIURL);
            if (!categoryRes.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load category.");
            }
            string categoryData = await categoryRes.Content.ReadAsStringAsync();
            var categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (categoryData);
  
            ViewData["ParentCategoryId"] = new SelectList( categoryResponse.Value, "CategoryId", "CategoryName", response.ParentCategoryId);
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(updateCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PutAsync($"{CategoryAPIURL}/{id}", content);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Server Error");
            }
            HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
            if (!Res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load specializations.");
            }
            string parentCategoryData = await Res.Content.ReadAsStringAsync();
            var parentCategoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (parentCategoryData);
            ViewBag.ParentCategory = new SelectList(parentCategoryResponse.Value, "CategoryId", "CategoryName");
            return View(updateCategoryDTO);

        }

        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.GetAsync($"{CategoryAPIURL}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Category>(rData);
            return View(response);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.DeleteAsync($"{CategoryAPIURL}/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<BadRequestModel>(rData);
            ModelState.AddModelError(string.Empty, response.Value);

            HttpResponseMessage res1 = await _httpClient.GetAsync($"{CategoryAPIURL}/{id}");
            if (!res1.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData1 = await res1.Content.ReadAsStringAsync();
            var response1 = JsonConvert.DeserializeObject<Category>(rData1);
            return View(response1);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.GetAsync($"{CategoryAPIURL}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Category>(rData);

            HttpResponseMessage Res = await _httpClient.GetAsync(CategoryAPIURL);
            if (!Res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to load specializations.");
            }
            string parentCategoryData = await Res.Content.ReadAsStringAsync();
            var parentCategoryResponse = JsonConvert.DeserializeObject<CategoryResponse>
            (parentCategoryData);
            ViewBag.ParentCategory = new SelectList(parentCategoryResponse.Value, "CategoryId", "CategoryName");
            return View(response);

        }


    }
}

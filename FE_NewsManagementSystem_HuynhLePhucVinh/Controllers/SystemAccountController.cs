using BusinessObject;
using DTO.Account;
using DTO.Category;
using FE_NewsManagementSystem_HuynhLePhucVinh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using UniversityMVC.Controllers;
using static FE_NewsManagementSystem_HuynhLePhucVinh.Controllers.CategoryController;

namespace FE_NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    public class SystemAccountController : BaseController
    {
        public SystemAccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
: base(httpClientFactory, configuration, httpContextAccessor) { }

        public class SystemAccountResponse
        {
            [JsonProperty("value")]
            public List<SystemAccount> Value { get; set; }
            [JsonProperty("@odata.count")]
            public int TotalCount { get; set; }
        }
        public async Task<IActionResult> Index(int? skip = 0, int? top = 2, string? searchString = null)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            string str = "";
            str = SystemAccountAPIURL;
            if (skip != null && top != null)
            {
                str += "?$skip=" + skip + "&$top=" + top + "&$count=true";
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                str += $"&$filter=contains(tolower(AccountName), '{searchString.ToLower()}')";
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
            var response = JsonConvert.DeserializeObject<SystemAccountResponse>(rData);

            int totalRecords = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / top.Value);
            ViewBag.CurrentPage = skip.Value / top.Value + 1;
            ViewBag.PageSize = top;
            ViewBag.SearchString = searchString ?? string.Empty;
            return View(response.Value);
        }

        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountDTO accountDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(accountDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync(SystemAccountAPIURL, content);
                //string responseContent = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                string data = await res.Content.ReadAsStringAsync();
                var responseError = JsonConvert.DeserializeObject<ErrorExceptionModel>(data);
                ModelState.AddModelError(string.Empty, responseError.Value);
            }
            return View(accountDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            string str = "";
            str = SystemAccountAPIURL;
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
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAccountDTO updateAccountDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(updateAccountDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PutAsync($"{SystemAccountAPIURL}/{id}", content);

                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                string data = await res.Content.ReadAsStringAsync();
                var responseError = JsonConvert.DeserializeObject<ErrorExceptionModel>(data);
                ModelState.AddModelError(string.Empty, responseError.Value);
            }
            string str = "";
            str = SystemAccountAPIURL;
            HttpResponseMessage res2 = await _httpClient.GetAsync($"{str}/{id}");
            if (!res2.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res2.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);

        }

        public async Task<IActionResult> EditProfile()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }          
            string str = "";
            str = SystemAccountAPIURL;
            HttpResponseMessage res = await _httpClient.GetAsync($"{str}/token-user");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateProfileDTO updateAccountDTO)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
            {
                return RedirectToAction("Login", "Auth");
            }
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(updateAccountDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PutAsync($"{SystemAccountAPIURL}/profile/{updateAccountDTO.AccountId}", content);

                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Logout", "Auth");
                }

                string data = await res.Content.ReadAsStringAsync();
                var responseError = JsonConvert.DeserializeObject<ErrorExceptionModel>(data);
                ModelState.AddModelError(string.Empty, responseError.Value);
            }
            string str = "";
            str = SystemAccountAPIURL;
            HttpResponseMessage res2 = await _httpClient.GetAsync($"{str}/token-user");
            if (!res2.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier
                });
            }
            string rData = await res2.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);

        }


        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.GetAsync($"{SystemAccountAPIURL}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.DeleteAsync($"{SystemAccountAPIURL}/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<BadRequestModel>(rData);
            ModelState.AddModelError(string.Empty, response.Value);
            HttpResponseMessage res1 = await _httpClient.GetAsync($"{SystemAccountAPIURL}/{id}");
            if (!res1.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData1 = await res1.Content.ReadAsStringAsync();
            var response1 = JsonConvert.DeserializeObject<SystemAccount>(rData1);
            return View(response1);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "3")
            {
                return RedirectToAction("Login", "Auth");
            }
            HttpResponseMessage res = await _httpClient.GetAsync($"{SystemAccountAPIURL}/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            }
            string rData = await res.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<SystemAccount>(rData);
            return View(response);

        }
    }
}

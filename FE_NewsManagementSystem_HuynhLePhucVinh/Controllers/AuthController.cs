using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using UniversityMVC.Controllers;

namespace FE_NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    public class AuthController : BaseController
    {
        public class LoginRequest
        {
            [JsonProperty("email")]
            public string? Email { get; set; }
            [JsonProperty("password")]
            public string? Password { get; set; }

        }
        public class AuthDTO
        {
            public bool IsAuthenticated { get; set; }
            public string? Token { get; set; }
            public DateTime? Expiration { get; set; }
            public string? Message { get; set; }
            public int? Role { get; set; }
            public string? Name { get; set; }
        }

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
: base(httpClientFactory, configuration, httpContextAccessor) { }
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SystemAccount sa)
        {
            if (ModelState.IsValid)
            {
                LoginRequest loginRequest = new LoginRequest
                {
                    Email = sa.AccountEmail,
                    Password = sa.AccountPassword
                };
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await _httpClient.PostAsync(AuthAPIURL + "/login", content);
                string responseContent = await res.Content.ReadAsStringAsync();
                var authResult = JsonConvert.DeserializeObject<AuthDTO>(responseContent);
                if (res.IsSuccessStatusCode)
                {


                    if (authResult.IsAuthenticated)
                    {
                        // Store token in session
                        HttpContext.Session.SetString("JWTToken", authResult.Token);
                        HttpContext.Session.SetString("UserRole", authResult.Role?.ToString() ?? "");
                        HttpContext.Session.SetString("Name", authResult.Name?.ToString() ?? "");
                        // You can also store the expiration if needed
                        HttpContext.Session.SetString("TokenExpiration", authResult.Expiration.ToString());
                        var role = HttpContext.Session.GetString("UserRole");
                        if (role == "1")
                        {
                            return RedirectToAction("Index", "Menu");
                        }
                        else if (role == "3") 
                        {
                            return RedirectToAction("Index", "SystemAccount");
                        }
                          
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, authResult.Message);
                    }
                }
                else
                {

                    ModelState.AddModelError(string.Empty, authResult.Message);
                }
            }
            return View(sa);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "NewsArticle");
        }

    }
}

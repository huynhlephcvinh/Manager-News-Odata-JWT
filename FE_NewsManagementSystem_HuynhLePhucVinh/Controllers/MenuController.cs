using Microsoft.AspNetCore.Mvc;
using UniversityMVC.Controllers;

namespace FE_NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    public class MenuController : BaseController
    {
        public MenuController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor) { }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("UserRole") == "1")
            {
                return View();
            }
            return RedirectToAction("Login", "Auth");
        }
    }
}

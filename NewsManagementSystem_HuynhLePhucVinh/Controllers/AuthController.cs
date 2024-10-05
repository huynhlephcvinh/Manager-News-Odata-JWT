using BusinessObject;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    [Route("odata/Auth")]
    [ApiController]
    public class AuthController : ODataController
    {
        private readonly ISystemAccountService _systemAccountService;

        public AuthController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        // POST api/<AuthController>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authResult =  _systemAccountService.Authenticate(loginDTO.Email,loginDTO.Password);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized(authResult);
            }

            return Ok(authResult);
        }

    }
}

using AutoMapper;
using BusinessObject;
using DTO.Account;
using DTO.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Services;

namespace NewsManagementSystem_HuynhLePhucVinh.Controllers
{
    [Route("odata/SystemAccounts")]
    [ApiController]
    public class SystemAccountsController : ODataController
    {
        public ISystemAccountService _systemAccountService;
        private readonly IMapper _mapper;
        public SystemAccountsController(ISystemAccountService systemAccountService, IMapper mapper)
        {
            _systemAccountService = systemAccountService;
            _mapper = mapper;
        }

        // GET: api/<CategoryController>
        [EnableQuery]
        [HttpGet("")]
        [Authorize(Roles = "3")]
        public ActionResult<IEnumerable<SystemAccount>> GetAll()
        {
            var sa = _systemAccountService.GetAllSystemAccount();
            //var responseCategoty = _mapper.Map<IEnumerable<ResponseCategoryDTO>>(categories);
            return Ok(sa);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{key}")]
        [Authorize(Roles = "3")]
        public ActionResult<SystemAccount> GetById([FromODataUri] short key)
        {
            var sa = _systemAccountService.GetSystemAccount(key);
            if (sa == null) { return NotFound(); }
            //var responseCategoty = _mapper.Map<ResponseCategoryDTO>(category);
            return Ok(sa);
        }

        // POST api/<CategoryController>
        [HttpPost("")]
        [Authorize(Roles = "3")]
        public IActionResult Post([FromBody] CreateAccountDTO createAccountDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var systemAccount = _mapper.Map<SystemAccount>(createAccountDTO);
                _systemAccountService.AddAccount(systemAccount);
                return Created(systemAccount);
            }
            catch (IOException ex) when (ex.Message == "Email exist!!")
            {

                return StatusCode(400, ex.Message);
            }
            catch (IOException ex)
            {

                return StatusCode(500, "An error occurred while processing your request");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{key}")]
        [Authorize(Roles = "3")]
        public ActionResult Put([FromODataUri] short key, [FromBody] UpdateAccountDTO updateAccountDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var exist = _systemAccountService.GetSystemAccount(key);
                if (exist == null)
                {
                    return NotFound();
                }
                var systemAccount = _mapper.Map<SystemAccount>(updateAccountDTO);
                systemAccount.AccountId = exist.AccountId;
                _systemAccountService.UpdaterAccount2(systemAccount);
                return Created(systemAccount);
            }
            catch (IOException ex) when (ex.Message == "Email exist!!")
            {
               
                return StatusCode(400, ex.Message);
            }
            catch (IOException ex)
            {
               
                return StatusCode(500, "An error occurred while processing your request");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }

        }

        [HttpPut("profile/{key}")]
        [Authorize(Roles = "1")]
        public ActionResult PutProfile([FromODataUri] short key, [FromBody] UpdateProfileDTO updateAccountDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = _systemAccountService.GetSystemAccount(key);
            if (exist == null)
            {
                return NotFound("Ko tim thay user");
            }
            var systemAccount = _mapper.Map<SystemAccount>(updateAccountDTO);
            systemAccount.AccountId = exist.AccountId;
            _systemAccountService.UpdaterAccount(systemAccount);
            systemAccount.AccountRole = 1;
            return Ok(systemAccount);

        }

        [HttpGet("token-user")]
        [Authorize(Roles = "1")]  // Only allow users with Role "1"
        public IActionResult GetUserProfileFromToken()
        {
            // Retrieve JWT token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Extract user ID from the token
            string userId = JwtGenerator.ExtractUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID could not be extracted from token.");
            }

            // Fetch user profile based on extracted user ID
            var userProfile = _systemAccountService.GetSystemAccount((short)int.Parse(userId));

            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(userProfile);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{key}")]
        [Authorize(Roles = "3")]
        public IActionResult Delete([FromODataUri] short key)
        {
            var tag = _systemAccountService.GetSystemAccount(key);
            if (tag == null)
            {
                return NotFound();
            }
           int delete =  _systemAccountService.RemoveAccount(key);
            if (delete == 0) { 
                return StatusCode(400,"Tài khoản này đã tạo news nên không thể xóa");
            }
            return Ok("Xóa thành công");

        }
    }
}

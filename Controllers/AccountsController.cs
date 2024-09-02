using BlogApp.Data;
using BlogApp.JwtFeatures;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly AppDbContext _appDbContext;
        public AccountsController(UserManager<User> userManager, JwtHandler jwtHandler, AppDbContext appDbContext)
        {
            this._userManager = userManager;
            this._jwtHandler = jwtHandler;
            this._appDbContext = appDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto input)
        {
            if(input == null)
            {
                return BadRequest();
            }
            var user = new User() { 
                Email = input.Email,
                UserName = input.UserName,
                FirstName = input.FirstName,
                LastName = input.LastName,
                DateOfBirth = input.DateOfBirth,
            };
            var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded) { 
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterUserResponseDto { Errors = errors});
            }
            return StatusCode(201);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserDto authenticateUserDto)
        {
            var user = await _userManager.FindByEmailAsync(authenticateUserDto.Email!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, authenticateUserDto.Password!)) { 
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid authentication credentials."});
            }
            var token = _jwtHandler.CreateToken(user);
            return Ok(new AuthResponseDto { IsAuthenticated = true, Token = token });
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            await _userManager.DeleteAsync(user);
            return Ok("User deleted successfully");
        }

        [HttpGet]
        [Route("{id:string}")]
        public IActionResult GetUserById(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                return NotFound("User not found");
            }
            var resp = new GetProfileDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                ProfilePhoto = user.ProfilePhoto,
                UserName = user.UserName
            };
            return Ok(resp);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAuthenticatedUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                return NotFound("User not found");
            }
            var resp = new GetMyProfileDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                ProfilePhoto = user.ProfilePhoto,
                UserName = user.UserName,
                Email = user.Email
            };
            return Ok(resp);
        }
    }
}

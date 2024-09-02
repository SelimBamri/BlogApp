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
            if (input.ProfilePhoto != null) { 
                user.ProfilePhoto = Convert.FromBase64String(input.ProfilePhoto);
            }
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
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            await _userManager.DeleteAsync(user);
            return Ok("User deleted successfully");
        }

        [HttpGet]
        [Route("{id}")]
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
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);

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

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile(UpdateMyProfileRequestDto input)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (input.FirstName != null)
            {
                user.FirstName = input.FirstName;
            }
            if (input.LastName != null)
            {
                user.LastName = input.LastName;
            }
            if (input.DateOfBirth != null)
            {
                user.DateOfBirth = input.DateOfBirth.Value;
            }
            if (input.Email != null)
            {
                user.Email = input.Email;
            }
            if (input.ProfilePhoto != null) {
                user.ProfilePhoto = Convert.FromBase64String(input.ProfilePhoto);
            }
            if (input.UserName != null)
            {
                user.UserName = input.UserName;
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            var token = _jwtHandler.CreateToken(user);
            return Ok(new { Token = token });
        }

        [HttpPut]
        [Route("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(UpdateMyPasswordDto input)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                var errors = passwordChangeResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            var token = _jwtHandler.CreateToken(user);
            return Ok(new { Token = token });
        }
    }
}

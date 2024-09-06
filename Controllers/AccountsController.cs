using BlogApp.Data;
using BlogApp.JwtFeatures;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                string base64Data = input.ProfilePhoto.Substring(input.ProfilePhoto.IndexOf(',') + 1);
                user.ProfilePhoto = Convert.FromBase64String(base64Data);
            }
            try
            {
                var result = await _userManager.CreateAsync(user, input.Password);
                if (!result.Succeeded) { 
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new RegisterUserResponseDto { Errors = errors});
                }
                return StatusCode(201);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return BadRequest(new RegisterUserResponseDto { Errors = ["The email '" + input.Email +"' is already taken." ] });
                }

                return StatusCode(500, "An unexpected error occurred.");
            }

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
            return Ok(new
            {
                Message = "ok"
            });
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
                string base64Data = input.ProfilePhoto.Substring(input.ProfilePhoto.IndexOf(',') + 1);
                user.ProfilePhoto = Convert.FromBase64String(base64Data);
            }
            if (input.UserName != null)
            {
                user.UserName = input.UserName;
            }
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new RegisterUserResponseDto { Errors = errors });
                }
                var token = _jwtHandler.CreateToken(user);
                return Ok(new { Token = token });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return BadRequest(new RegisterUserResponseDto { Errors = ["The email '" + input.Email + "' is already taken."] });
                }

                return StatusCode(500, "An unexpected error occurred.");
            }
            
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

using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public AccountsController(UserManager<User> userManager)
        {
            this._userManager = userManager;
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
    }
}

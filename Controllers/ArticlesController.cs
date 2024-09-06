using BlogApp.Data;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public ArticlesController(AppDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddArticle(AddArticleDto input)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            string base64Data = input.Banner.Substring(input.Banner.IndexOf(',') + 1);
            Article article = new Article()
            {
                Author = user,
                Title = input.Title,
                Banner = Convert.FromBase64String(base64Data),
                Content = input.Content,
                Description = input.Description,
                Created = DateTime.Now
            };
            _context.Articles.Add(article);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Article created successfully"
            });
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(int id, UpdateArticleDto input)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Article article = _context.Articles.Find(id);
            if (article == null)
            {
                return NotFound("Article not found");
            }
            if (article.Author != user)
            {
                return Unauthorized("You don't have the right to edit this article");
            }
            if(input.Title != null) { 
                article.Title = input.Title;
            }
            if (input.Description != null)
            {
                article.Description = input.Description;
            }
            if (input.Content != null)
            {
                article.Content = input.Content;
            }
            if (input.Banner != null)
            {
                string base64Data = input.Banner.Substring(input.Banner.IndexOf(',') + 1);
                article.Banner = Convert.FromBase64String(base64Data);
            }
            _context.SaveChanges();
            return Ok(new
            {
                message = "Profile updated successfully"
            });
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Article article = _context.Articles.Find(id);
            if (article == null)
            {
                return NotFound("Article not found");
            }
            if (article.Author != user)
            {
                return Unauthorized("You don't have the right to edit this article");
            }
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Article deleted successfully."
            });
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyArticlesByPage([FromQuery] int page = 1)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            int pageSize = 5;
            var resp = _context.Articles
                .Where(a => a.Author == user)
                .OrderBy(a => a.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new GetArticleDto()
                {
                    Id = a.Id,
                    Content = a.Content,
                    Title = a.Title,
                    Description = a.Description,
                    Created = a.Created,
                    Banner = Convert.ToBase64String(a.Banner)
                })
                .ToList();
            return Ok(new
            {
                results = resp
            });
        }

        [HttpGet]
        [Route("info")]
        [Authorize]
        public async Task<IActionResult> GetMyPageInfo()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }
            int count = _context.Articles
                                .Where(a => a.Author == user)
                                .Count();
            if(count <= 5)
            {
                return Ok(new
                {
                    LastPage = 1
                });
            }
            return Ok(new
            {
                LastPage = 1 + count/5
            });
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetArticleById(int id)
        {
            Article article = _context.Articles.Find(id);
            if (article == null)
            {
                return BadRequest();
            }
            GetArticleDto resp = new GetArticleDto()
            {
                Id = article.Id,
                Content = article.Content,
                Title = article.Title,
                Description = article.Description,
                Created = article.Created,
                Banner = Convert.ToBase64String(article.Banner)
            };
            return Ok(resp);
        }
    }
}

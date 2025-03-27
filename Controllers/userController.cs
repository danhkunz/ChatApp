using ChatApp.Caches;
using ChatApp.ChatHubs;
using ChatApp.DBContext;
using ChatApp.Dtos;
using ChatApp.Helpers;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    //[Authorize]
    public class userController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;

        private readonly Cache _cache;

        private IUserService _userService;

        private readonly IConfiguration _configuration;

        public userController(IHubContext<ChatHub> chatHub, IUserService userService, IConfiguration configuration)
        {
            _chatHub = chatHub;
            _userService = userService;
            _configuration = configuration;
            _cache = new Cache("AppCatch");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _userService.GetUserByName(loginDto.UserName);

            bool verify = false;
            try
            {
                verify = BCrypt.Net.BCrypt.EnhancedVerify(loginDto.Password, user.Password, hashType: BCrypt.Net.HashType.SHA256);
            }
            catch { }
            if (verify && user != null)
            {
                var token = Utils.GenerateAccessToken(user.Id.ToString(), _configuration["Jwt:Key"]);

                await _cache.UpdateAsync(token, user.Id.ToString(), 1);

                var CookieOpt = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                };

                HttpContext.Response.Cookies.Delete(Const.ACCESS_TOKEN, CookieOpt);
                HttpContext.Response.Cookies.Append(Const.ACCESS_TOKEN, token, CookieOpt);

                return Ok(user.Id);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            return Ok(await _userService.AddUser(user));
        }

        [HttpGet]
        public async Task<IActionResult> GetListUser()
        {
            return Ok(await _userService.GetListUser());
        }
    }


}
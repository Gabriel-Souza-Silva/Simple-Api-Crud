using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCrud.Data;
using SimpleCrud.Models;
using SimpleCrud.Models.Request;
using SimpleCrud.Service.User;
using SimpleCrud.Services.JwtService;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace SimpleCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService, IUserService userService, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _jwtService = jwtService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("me"),Authorize]
        public async Task<ActionResult<User>> GetMe()
        {
            var userId = GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return Ok(user);
        }

        [HttpPost("verify")]
        public ActionResult<User> Verify(string token)
        {
            string error = _userService.Verify(token);

            if (error.IsNullOrEmpty() is false)
                return BadRequest(error);

            return Ok("User verified");
        }

        [HttpPost("login")]
        public ActionResult<string> Login(LoginUserRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(request.Email));

            if (user is null)
            {
                return BadRequest("Usuário não encontrado");
            }

            if (!_jwtService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Email ou senha incorretos");
            }

            string token = _jwtService.CreatedToken(user);

            var refreshToken = _jwtService.GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            _context.SaveChanges();

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public ActionResult<string> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if(user is null)
            {
                return Unauthorized("Invalid refresh token");
            }
            else if(user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired");
            }

            string token = _jwtService.CreatedToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, user);

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(token);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return BadRequest("User not found");
            }

            user.PasswordResetToken = _jwtService.CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(user.PasswordResetToken);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == resetPasswordRequest.Token);

            if (user is null)
            {
                return BadRequest("User not found");
            }

            (user.PasswordHash, user.PasswordSalt) = _jwtService.CreatePasswordHash(resetPasswordRequest.Password);

            user.ResetTokenExpires = null;
            user.PasswordResetToken = null;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok("Check your email");
        }

        private void SetRefreshToken(RefreshToken refreshToken, User user)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOption);

            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}

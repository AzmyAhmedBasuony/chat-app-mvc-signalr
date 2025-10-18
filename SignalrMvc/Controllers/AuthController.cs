using Microsoft.AspNetCore.Mvc;
using SignalrMvc.DTOs;
using SignalrMvc.Services;
using SignalrMvc.UnitOfWork;

namespace SignalrMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public AuthController(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("api/auth/login")]
        public async Task<ActionResult<LoginResponseDto>> LoginApi([FromBody] LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _authService.GenerateJwtToken(user);

            return Ok(new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                ProfileImage = user.ProfileImage,
                Token = token
            });
        }

        [HttpGet("api/auth/users")]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                ProfileImage = u.ProfileImage,
                IsOnline = u.IsOnline,
                LastSeen = u.LastSeen
            }).ToList();

            return Ok(userDtos);
        }
    }
}


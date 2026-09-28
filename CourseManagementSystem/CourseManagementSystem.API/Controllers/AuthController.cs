using CourseManagementSystem.Application.DTOs.User;
using CourseManagementSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginUserDto dto)
        {
            var response =
                await _authService.LoginAsync(dto);

            return Ok(response);
        }
    }
}
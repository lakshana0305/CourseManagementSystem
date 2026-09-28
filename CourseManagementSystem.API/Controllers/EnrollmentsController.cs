using System.Security.Claims;
using CourseManagementSystem.Application.DTOs.Enrollment;
using CourseManagementSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(
            IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(
            CreateEnrollmentDto dto)
        {
            var userId = GetCurrentUserId();

            dto.UserId = userId;

            var enrollment =
                await _enrollmentService.EnrollAsync(dto);

            return Ok(enrollment);
        }

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = GetCurrentUserId();

            var enrollments =
                await _enrollmentService
                    .GetMyEnrollmentsAsync(userId);

            return Ok(enrollments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(
            int id)
        {
            var userId = GetCurrentUserId();

            await _enrollmentService
                .CancelEnrollmentAsync(id, userId);

            return Ok(
                "Enrollment cancelled successfully.");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException(
                    "User ID was not found in the token.");
            }

            return int.Parse(userIdClaim);
        }
    }
}
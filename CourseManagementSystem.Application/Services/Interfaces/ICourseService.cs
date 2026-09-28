using CourseManagementSystem.Application.DTOs.Course;
using CourseManagementSystem.Domain.Entities;

namespace CourseManagementSystem.Application.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllAsync();

        Task<Course?> GetByIdAsync(int id);

        Task<Course> AddAsync(CreateCourseDto dto);

        Task UpdateAsync(int id, UpdateCourseDto dto);

        Task DeleteAsync(int id);
    }
}
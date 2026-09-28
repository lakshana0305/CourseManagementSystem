using CourseManagementSystem.Application.DTOs.Course;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;

namespace CourseManagementSystem.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task<Course> AddAsync(CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Instructor = dto.Instructor
            };

            return await _courseRepository.AddAsync(course);
        }

        public async Task UpdateAsync(
            int id,
            UpdateCourseDto dto)
        {
            var existingCourse =
                await _courseRepository.GetByIdAsync(id);

            if (existingCourse == null)
            {
                throw new Exception("Course not found.");
            }

            existingCourse.Title = dto.Title;
            existingCourse.Description = dto.Description;
            existingCourse.Instructor = dto.Instructor;

            await _courseRepository.UpdateAsync(existingCourse);
        }

        public async Task DeleteAsync(int id)
        {
            var course =
                await _courseRepository.GetByIdAsync(id);

            if (course == null)
            {
                throw new Exception("Course not found.");
            }

            await _courseRepository.DeleteAsync(course);
        }
    }
}
namespace CourseManagementSystem.Application.DTOs.Course
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Instructor { get; set; } = string.Empty;
    }
}
using Dotnet_Assignment_SpringCT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Assignment_SpringCT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.Include(s => s.StudentCourses).ThenInclude(
                sc => sc.Course).
                Select(s => new
                {
                    s.Name,
                    s.Email,
                    s.Phone,
                    Courses = string.Join(",", s.StudentCourses.Select(sc => sc.Course.Name))
                })
                .ToListAsync();

            return Ok(students);
        }
        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return Ok(student);
        }
        [HttpPost("{studentId/courses}")]
        public async Task<IActionResult> AssignCourses(int studentId, [FromBody] List<int> courseIds)
        {
            var student = await _context.Students.Include(s => s.StudentCourses)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }

            foreach (var courseId in courseIds)
            {
                if(!student.StudentCourses.Any(sc => sc.CourseId == courseId))
                {
                    student.StudentCourses.Add(new StudentCourse { StudentId = studentId,CourseId = courseId });
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

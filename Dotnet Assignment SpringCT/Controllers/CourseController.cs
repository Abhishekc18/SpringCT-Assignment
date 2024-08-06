using Dotnet_Assignment_SpringCT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Assignment_SpringCT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{courseName}/students")]
        public async Task<IActionResult> GetStudentByCourse(string courseName)
        {
            var course = await _context.Courses.Include(c => c.StudentCourses).ThenInclude(c => c.Student).FirstOrDefaultAsync(c => c.Name == courseName);

            if (course == null) { 
                return NotFound();
            }
            var students = course.StudentCourses.Select(sc =>
            new { sc.Student.Name, sc.Student.Email, sc.Student.Phone }).ToList();
            return Ok(students);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return Ok(course);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}

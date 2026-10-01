using ExamApiDemo.Interfaces;
using ExamApiDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

  [HttpGet]
public async Task<IActionResult> AllCourse(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    if (page < 1)
    {
        return BadRequest(new
        {
            message =
            "page must be greater than or equal to 1"
        });
    }

    if (pageSize < 1 || pageSize > 100)
    {
        return BadRequest(new
        {
            message =
            "pageSize must be between 1 and 100"
        });
    }

    var result =
        await _courseService.GetAllAsync(    page,     pageSize);

    return Ok(result);
}

    [HttpGet("{courseno}")]
    public async Task<IActionResult> Courseno(int courseno)
    {
        var course =
            await _courseService.GetByIdAsync(courseno);

        if (course == null)
        {
            return NotFound(new
            {
                message = "No results found"
            });
        }

        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> InsertCourse(
        [FromBody] CourseModel course)
    {
        if (
            course.Courseno <= 0 ||
            string.IsNullOrWhiteSpace(course.CourseCode) ||
            string.IsNullOrWhiteSpace(course.CourseName))
        {
            return BadRequest(new
            {
                message =
                "courseno must be greater than zero and Course Name and Course Code cannot be null"
            });
        }

        bool exists =
            await _courseService.ExistsAsync(course.Courseno);

        if (exists)
        {
            return Conflict(new
            {
                message = "Course already exists"
            });
        }

        int rows =
            await _courseService.InsertAsync(course);

        if (rows == 0)
        {
            return StatusCode(
                500,
                new
                {
                    message = "Insert failed"
                });
        }

        return Created(
            $"/api/course/{course.Courseno}",
            course);
    }

    [HttpPut("{courseno}")]
    public async Task<IActionResult> UpdateCourse(
        int courseno,
        [FromBody] CourseModel course)
    {
        if (
            courseno <= 0 ||
            string.IsNullOrWhiteSpace(course.CourseCode) ||
            string.IsNullOrWhiteSpace(course.CourseName))
        {
            return BadRequest(new
            {
                message =
                "courseno must be greater than zero and Course Name and Course Code cannot be null"
            });
        }

        bool exists =
            await _courseService.ExistsAsync(courseno);

        if (!exists)
        {
            return NotFound(new
            {
                message = "Course Not Found"
            });
        }

        int rows =
            await _courseService.UpdateAsync(
                courseno,
                course);

        if (rows == 0)
        {
            return StatusCode(
                500,
                new
                {
                    message = "Update failed"
                });
        }

        return Ok(course);
    }

    [HttpDelete("{courseno}")]
    public async Task<IActionResult> DeleteCourse(
        int courseno)
    {
        if (courseno <= 0)
        {
            return BadRequest(new
            {
                message = "courseno must be valid"
            });
        }

        int rows =
            await _courseService.DeleteAsync(courseno);

        if (rows == 0)
        {
            return NotFound(new
            {
                message = "Course Not Found"
            });
        }

        return NoContent();
    }
}
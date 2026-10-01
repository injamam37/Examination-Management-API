using ExamApiDemo.Interfaces;
using ExamApiDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamResultController : ControllerBase
{
    private readonly IExamResultService _examResultService;

    public ExamResultController(
        IExamResultService examResultService)
    {
        _examResultService = examResultService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentResults(
        [FromQuery] int idno,
        [FromQuery] int sessionno)
    {
        if (
            idno <= 0 ||
            sessionno <= 0)
        {
            return BadRequest(new
            {
                message =
                "Idno and Sessionno must be greater than zero."
            });
        }

        var result =
            await _examResultService.GetStudentResultsAsync(
                idno,
                sessionno);

        if (!result.Any())
        {
            return NotFound(new
            {
                message = "No results found"
            });
        }

        return Ok(result);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateResult(
        [FromBody] CalculateResultRequest request)
    {
        if (
            request.Idno <= 0 ||
            request.Sessionno <= 0 ||
            request.Courseno <= 0 ||
            request.Semesterno <= 0)
        {
            return BadRequest(new
            {
                message =
                "Idno, Sessionno, Courseno and Semesterno must be greater than zero."
            });
        }

        var result =
            await _examResultService.CalculateResultAsync(
                request);

        if (result == null)
        {
            return NotFound(new
            {
                message = "No result found"
            });
        }

        return Ok(result);
    }
}
using Microsoft.AspNetCore.Mvc;

namespace ExamApiDemo.Controllers;

[ApiController]

[Route("api/[controller]")]

public class StudentController   : ControllerBase
{

    [HttpGet]

    public IActionResult Get()
    {
        return Ok(
            "Get Executed"
        );
    }


    [HttpGet("details")]

    public IActionResult Details()
    {
        return Ok(
            "Details Executed"
        );
    }

    [HttpGet("{id}")]

    public IActionResult GetById( int id )
    {
        return Ok(
            "Student Id = " + id
        );
    }

}
using Microsoft.AspNetCore.Mvc;
using ExamApiDemo.Models;
using Microsoft.Data.SqlClient;


namespace ExamApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamController : ControllerBase
{
    private readonly string _connString;

  public ExamController(string connString)
    {
        _connString = connString;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "Exam API is running",
            module = "Examination"
        });
    }

    [HttpGet("ping-db")]
    public async Task<IResult> PingDb()
    {
        await using var connection =
            new SqlConnection(_connString);

        await connection.OpenAsync();

        await using var command =
            new SqlCommand(
                "SELECT GETDATE()",
                connection);

        var result =
            await command.ExecuteScalarAsync();

        return Results.Json(new
        {
            serverTime =
                Convert.ToDateTime(result),

            status = "connected"
        });
    }

[HttpGet("studentresult")]
    public async Task<IActionResult> GetStudents(
        [FromQuery] int idno,
        [FromQuery] int sessionno)
    {
        var list = new List<object>();

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
             SELECT idno,sessionno,sr.semesterno,sr.schemeno,
                sr.courseno,
                sr.INTERNAL,
                sr.[EXTERNAL],
                TOTALMARK,
                iif(isnull(INTERNAL,0)<isnull(MININTERNAL,0) or isnull([EXTERNAL],0)<isnull(MINEXTERNAL,0) or isnull(TOTALMARK,0)<isnull(c.MINTOTAL,0),'Fail','Pass') passfail
            FROM STUDMARK sr
            INNER JOIN TESTCOURSE c ON c.courseno = sr.courseno
            WHERE idno = @idno
            AND sessionno = @sessionno";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@idno", idno);
        cmd.Parameters.AddWithValue("@sessionno", sessionno);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new
            {
                courseno = reader["courseno"],
                internalMarks = reader["INTERNAL"] as decimal?,
                externalMarks = reader["EXTERNAL"] as decimal?,
                total = reader["TOTALMARK"] as decimal?,
                passfail = reader["passfail"]
            });
        }

        if (!list.Any())
        {
            return NotFound(new
            {
                message = "No results found"
            });
        }

        return Ok(list);
    }
[HttpPost("mark-entry")]
    public async Task<IActionResult> InsertMarkEntry(
        [FromBody] ExamMarkEntry entry)
    {
        if (entry.Idno <= 0 || entry.Courseno <= 0)
        {
            return BadRequest(new
            {
                message = "idno and courseno must be greater than zero"
            });
        }

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
INSERT INTO STUDMARK
(
    IDNO,
    SESSIONNO,
    COURSENO,
    SEMESTERNO,
    INTERNAL,
    [EXTERNAL]
)
VALUES
(
    @idno,
    @sessionno,
    @courseno,
    @semesterno,
    @internal,
    @external
)";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@idno", entry.Idno);
        cmd.Parameters.AddWithValue("@sessionno", entry.Sessionno);
        cmd.Parameters.AddWithValue("@courseno", entry.Courseno);
        cmd.Parameters.AddWithValue("@semesterno", entry.Semesterno);
        cmd.Parameters.AddWithValue("@internal", (object?)entry.InternalMarks ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@external", (object?)entry.ExternalMarks ?? DBNull.Value);

        int rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0)
        {
            return StatusCode(500, new { message = "Insert failed" });
        }

        return Created("/api/exam/mark-entry", entry);
    }
    [HttpPut("mark-entry/{idno}/{courseno}/{sessionno}")]
    public async Task<IActionResult> UpdateMark(
        int idno,
        int courseno,
        int sessionno,
        [FromBody] ExamMarkEntry entry)
    {
        if (idno <= 0 || courseno <= 0 || sessionno <= 0)
        {
            return BadRequest(new
            {
                message = "Idno, courseno and sessionno must be greater than zero"
            });
        }

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string checkQuery =
        @"
        SELECT COUNT(*)
        FROM STUDMARK
        WHERE idno=@idno and courseno=@courseno and sessionno=@sessionno
        ";

        await using var checkCmd =
            new SqlCommand(checkQuery, conn);

        checkCmd.Parameters.AddWithValue("@idno", idno);
        checkCmd.Parameters.AddWithValue("@courseno", courseno);
        checkCmd.Parameters.AddWithValue("@sessionno", sessionno);

        int exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

        if (exists == 0)
        {
            return NotFound(new
            {
                message = "Mark entry not found"
            });
        }

        string query =
        @"
        UPDATE STUDMARK
        SET
            INTERNAL=iif(@internal is null,INTERNAL,@internal),
            [EXTERNAL]=iif(@external is null,[EXTERNAL],@external)
        WHERE idno=@idno and courseno=@courseno and sessionno=@sessionno
        ";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@idno", idno);
        cmd.Parameters.AddWithValue("@courseno", courseno);
        cmd.Parameters.AddWithValue("@sessionno", sessionno);
        cmd.Parameters.AddWithValue("@internal", entry.InternalMarks ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@external", entry.ExternalMarks ?? (object)DBNull.Value);

        int rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0)
        {
            return StatusCode(500, new { message = "Update failed" });
        }

        return Ok(new
        {
            idno = idno,
            courseno = courseno,
            sessionno = sessionno,
            internalMarks = entry.InternalMarks,
            externalMarks = entry.ExternalMarks
        });
    }

    [HttpDelete("mark-entry/{idno}/{courseno}/{sessionno}")]
    public async Task<IActionResult> CancelMarkEntry(
        int idno,
        int courseno,
        int sessionno)
    {
        if (idno <= 0 || courseno <= 0 || sessionno <= 0)
        {
            return BadRequest(new
            {
                message = "Idno, courseno and sessionno must be greater than zero"
            });
        }

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query =
        @"
        UPDATE STUDMARK SET cancel=1
        WHERE idno=@idno and courseno=@courseno and sessionno=@sessionno
        ";

        await using var deleteCmd =
            new SqlCommand(query, conn);

        deleteCmd.Parameters.AddWithValue("@idno", idno);
        deleteCmd.Parameters.AddWithValue("@courseno", courseno);
        deleteCmd.Parameters.AddWithValue("@sessionno", sessionno);

        int rows = await deleteCmd.ExecuteNonQueryAsync();

        if (rows == 0)
        {
            return NotFound(new
            {
                message = "Record not found"
            });
        }

        return NoContent();
    }


}
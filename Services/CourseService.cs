using ExamApiDemo.Interfaces;
using ExamApiDemo.Models;
using Microsoft.Data.SqlClient;

namespace ExamApiDemo.Services;

public class CourseService : ICourseService
{
    private readonly string _connString;

    public CourseService(string connString)
    {
        _connString = connString;
    }
    public async Task<PagedCourseResult> GetAllAsync(int page, int pageSize)
    {
        var result =
            new PagedCourseResult();

        result.Page = page;
        result.PageSize = pageSize;

        int offset =
            (page - 1) * pageSize;

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string countQuery =
        @"
    SELECT COUNT(*)
    FROM TESTCOURSE";

        await using var countCmd =
            new SqlCommand(
                countQuery,
                conn);

        result.TotalRecords =
            Convert.ToInt32(
                await countCmd.ExecuteScalarAsync());

        result.TotalPages =
            (int)Math.Ceiling(
                (double)result.TotalRecords
                / pageSize);

        string query =
        @"
    SELECT
        courseno,
        coursecode,
        coursename
    FROM TESTCOURSE
    ORDER BY courseno
    OFFSET @offset ROWS
    FETCH NEXT @pageSize ROWS ONLY";

        await using var cmd =
            new SqlCommand(
                query,
                conn);

        cmd.Parameters.AddWithValue(
            "@offset",
            offset);

        cmd.Parameters.AddWithValue(
            "@pageSize",
            pageSize);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Data.Add(
                new CourseModel
                {
                    Courseno =
                        Convert.ToInt32(
                            reader["courseno"]),

                    CourseCode =
                        reader["coursecode"].ToString(),

                    CourseName =
                        reader["coursename"].ToString()
                });
        }

        return result;

    }

    public async Task<CourseModel?> GetByIdAsync(int courseno)
    {
        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
SELECT
    courseno,
    coursecode,
    coursename
FROM TESTCOURSE
WHERE courseno=@courseno";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@courseno", courseno);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new CourseModel
            {
                Courseno = Convert.ToInt32(reader["courseno"]),
                CourseCode = reader["coursecode"].ToString(),
                CourseName = reader["coursename"].ToString()
            };
        }

        return null;
    }

    public async Task<bool> ExistsAsync(int courseno)
    {
        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
SELECT COUNT(*)
FROM TESTCOURSE
WHERE courseno=@courseno";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@courseno", courseno);

        int count =
            Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return count > 0;
    }

    public async Task<int> InsertAsync(CourseModel course)
    {
        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
INSERT INTO TESTCOURSE
(
    courseno,
    coursecode,
    coursename
)
VALUES
(
    @courseno,
    @coursecode,
    @coursename
)";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@courseno", course.Courseno);
        cmd.Parameters.AddWithValue("@coursecode", course.CourseCode);
        cmd.Parameters.AddWithValue("@coursename", course.CourseName);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> UpdateAsync(int courseno, CourseModel course)
    {
        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
UPDATE TESTCOURSE
SET
    COURSECODE=@coursecode,
    COURSENAME=@coursename
WHERE
    COURSENO=@courseno";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@courseno", courseno);
        cmd.Parameters.AddWithValue("@coursecode", course.CourseCode);
        cmd.Parameters.AddWithValue("@coursename", course.CourseName);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteAsync(int courseno)
    {
        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
DELETE
FROM TESTCOURSE
WHERE courseno=@courseno";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@courseno", courseno);

        return await cmd.ExecuteNonQueryAsync();
    }
}
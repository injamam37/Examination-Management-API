using ExamApiDemo.Interfaces;
using ExamApiDemo.Models;
using Microsoft.Data.SqlClient;

namespace ExamApiDemo.Services;

public class ExamResultService : IExamResultService
{
    private readonly string _connString;

    public ExamResultService(string connString)
    {
        _connString = connString;
    }

    public async Task<List<ExamResultModel>> GetStudentResultsAsync(
        int idno,
        int sessionno)
    {
        var list = new List<ExamResultModel>();

        await using var conn =
            new SqlConnection(_connString);

        await conn.OpenAsync();

        string query = @"
SELECT
    sr.IDNO,
    sr.SESSIONNO,
    sr.COURSENO,
    sr.INTERNAL,
    sr.[EXTERNAL],
    sr.TOTALMARK,
    sr.PASSFAIL,
    sr.GRADE
FROM STUDMARK sr
WHERE
    sr.IDNO=@idno
AND sr.SESSIONNO=@sessionno
AND ISNULL(sr.CANCEL,0)=0";

        await using var cmd =
            new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue(
            "@idno",
            idno);

        cmd.Parameters.AddWithValue(
            "@sessionno",
            sessionno);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(
                new ExamResultModel
                {
                    Idno =
                        Convert.ToInt32(reader["IDNO"]),

                    Sessionno =
                        Convert.ToInt32(reader["SESSIONNO"]),

                    Courseno =
                        Convert.ToInt32(reader["COURSENO"]),

                    InternalMarks =
                        reader["INTERNAL"] as decimal?,

                    ExternalMarks =
                        reader["EXTERNAL"] as decimal?,

                    Total =
                        reader["TOTALMARK"] as decimal?,

                    PassFail =
                        reader["PASSFAIL"].ToString(),

                    Grade =
                        reader["GRADE"].ToString()
                });
        }

        return list;
    }


    public async Task<ExamResultModel?> CalculateResultAsync(
    CalculateResultRequest request)
{
    await using var conn =
        new SqlConnection(_connString);

    await conn.OpenAsync();

    string query = @"
SELECT
    m.IDNO,
    m.SESSIONNO,
    m.COURSENO,
    m.INTERNAL,
    m.[EXTERNAL],
    c.MININTERNAL,
    c.MINEXTERNAL,
    c.MINTOTAL
FROM STUDMARK m
INNER JOIN TESTCOURSE  c
    ON c.COURSENO = m.COURSENO
WHERE
    m.IDNO=@idno
AND m.SESSIONNO=@sessionno
AND m.COURSENO=@courseno
AND m.SEMESTERNO=@semesterno
AND ISNULL(m.CANCEL,0)=0";

    await using var cmd =
        new SqlCommand(query, conn);

    cmd.Parameters.AddWithValue("@idno", request.Idno);
    cmd.Parameters.AddWithValue("@sessionno", request.Sessionno);
    cmd.Parameters.AddWithValue("@courseno", request.Courseno);
    cmd.Parameters.AddWithValue("@semesterno", request.Semesterno);

    await using var reader =
        await cmd.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
    {
        return null;
    }

    decimal? internalMark =
        reader["INTERNAL"] as decimal?;

    decimal? externalMark =
        reader["EXTERNAL"] as decimal?;

        decimal minInternal =reader["MININTERNAL"] as decimal? ?? 0;
        decimal minExternal =reader["MINEXTERNAL"] as decimal? ?? 0;
        decimal minTotal =reader["MINTOTAL"] as decimal? ?? 0;

    decimal total =
        (internalMark ?? 0)
        + (externalMark ?? 0);

    string passFail =
        (
            (internalMark ?? 0) < minInternal
            ||
            (externalMark ?? 0) < minExternal
            ||
            total < minTotal
        )
        ? "Fail"
        : "Pass";

    string grade =
        passFail == "Pass"
        ? "P"
        : "F";

    await reader.CloseAsync();

    string updateQuery = @"
UPDATE STUDMARK
SET
    TOTALMARK=@total,
    PASSFAIL=@passfail,
    GRADE=@grade
WHERE
    IDNO=@idno
AND SESSIONNO=@sessionno
AND COURSENO=@courseno
AND SEMESTERNO=@semesterno";

    await using var updateCmd =
        new SqlCommand(updateQuery, conn);

    updateCmd.Parameters.AddWithValue("@total", total);
    updateCmd.Parameters.AddWithValue("@passfail", passFail);
    updateCmd.Parameters.AddWithValue("@grade", grade);

    updateCmd.Parameters.AddWithValue("@idno", request.Idno);
    updateCmd.Parameters.AddWithValue("@sessionno", request.Sessionno);
    updateCmd.Parameters.AddWithValue("@courseno", request.Courseno);
    updateCmd.Parameters.AddWithValue("@semesterno", request.Semesterno);

    await updateCmd.ExecuteNonQueryAsync();

    return new ExamResultModel
    {
        Idno = request.Idno,
        Sessionno = request.Sessionno,
        Courseno = request.Courseno,
        InternalMarks = internalMark,
        ExternalMarks = externalMark,
        Total = total,
        PassFail = passFail,
        Grade = grade
    };
}
}
using System.ComponentModel.DataAnnotations;

namespace ExamApiDemo.Models;

public class ExamResultModel
{
    public int Idno { get; set; }

    public int Sessionno { get; set; }

    public int Courseno { get; set; }

    public decimal? InternalMarks { get; set; }

    public decimal? ExternalMarks { get; set; }

    public decimal? Total { get; set; }

    public string? PassFail { get; set; }

    public string? Grade { get; set; }
}
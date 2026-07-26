using System.ComponentModel.DataAnnotations;

namespace ExamApiDemo.Models;

public class ExamMarkEntry
{
    [Required]
    public int Idno { get; set; }

    [Required]
    public int Sessionno { get; set; }

    [Required]
    public int Courseno { get; set; }

    [Required]
    public int Semesterno { get; set; }

    public decimal? InternalMarks { get; set; }

    public decimal? ExternalMarks { get; set; }
}
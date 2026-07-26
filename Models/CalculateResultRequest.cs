using System.ComponentModel.DataAnnotations;

namespace ExamApiDemo.Models;

public class CalculateResultRequest
{
    [Required]
    public int Idno { get; set; }

    [Required]
    public int Sessionno { get; set; }

    [Required]
    public int Courseno { get; set; }

    [Required]
    public int Semesterno { get; set; }

}
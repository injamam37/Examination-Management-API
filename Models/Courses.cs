using System.ComponentModel.DataAnnotations;

namespace ExamApiDemo.Models;

public class CourseModel
{
    [Required]
    public int Courseno { get; set; }

    [Required]
    public string? CourseCode { get; set; }

    [Required]
    public string? CourseName { get; set; }

}
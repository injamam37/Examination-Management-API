namespace ExamApiDemo.Models;

public class PagedCourseResult
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public List<CourseModel> Data { get; set; } = new();
}
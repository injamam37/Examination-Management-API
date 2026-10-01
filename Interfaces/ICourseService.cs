using ExamApiDemo.Models;

namespace ExamApiDemo.Interfaces;

public interface ICourseService
{
    Task<PagedCourseResult> GetAllAsync(
        int page,
        int pageSize);


    Task<CourseModel?> GetByIdAsync(int courseno);

    Task<bool> ExistsAsync(int courseno);

    Task<int> InsertAsync(CourseModel course);

    Task<int> UpdateAsync(int courseno, CourseModel course);

    Task<int> DeleteAsync(int courseno);
}
using ExamApiDemo.Models;

namespace ExamApiDemo.Interfaces;

public interface IExamResultService
{
    Task<List<ExamResultModel>> GetStudentResultsAsync(
        int idno,
        int sessionno);

    Task<ExamResultModel?> CalculateResultAsync(
        CalculateResultRequest request);
}
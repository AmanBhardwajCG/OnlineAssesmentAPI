using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using OnlineAssesmentAPI.ModelClass;
using OnlineAssesmentAPI.ModelClass.ExamModel;
using System.Data;

namespace OnlineAssesmentAPI.Interface
{
    public interface IExamRepository
    {
        Task<long> CreateExamAsync(CreateExamRequest request, long createdByUserId);
        Task<(bool IsSuccess, string Message)> AssignCollegeAsync(AssignExamCollegeRequest request);

        Task<List<EnrollStudentResponse>> GetEligibleStudentsAsync(long examId);

        Task<StudentExamDTO> GetStudentExamsAsync(string email, string rollnumber);

        Task<bool> PublishExamAsync(ExamReview Review);

    }
}

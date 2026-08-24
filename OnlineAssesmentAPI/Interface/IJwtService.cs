using OnlineAssesmentAPI.ModelClass.ExamModel;

namespace OnlineAssesmentAPI.Interface
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string name, string role);
        string? GenerateStudentToken(StudentExamResponse response);
    }
}

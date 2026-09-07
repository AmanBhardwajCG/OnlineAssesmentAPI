using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Interface
{
    public interface IQuestionRepository
    {
        //Task<long> CreateQuestionAsync(Question question, long createdByUserId);
        //Task<bool> PublishQuestionAsync(QuestionReview Review);

        Task<string> UploadMcq(IFormFile file, long createdByUserId);
        Task<string> UploadCoding(IFormFile file, long createdByUserId);
        Task<List<Question>> GetAllMcqQuestions();


    }
}

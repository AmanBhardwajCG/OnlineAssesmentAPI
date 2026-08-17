using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Interface
{
    public interface IQuestionRepository
    {
        Task<long> CreateQuestionAsync(Question question, long createdByUserId);
        


    }
}

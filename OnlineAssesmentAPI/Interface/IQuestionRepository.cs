namespace OnlineAssesmentAPI.Interface
{
    public interface IQuestionRepository
    {
        Task<string> UploadMcq(IFormFile file);
        Task<string> UploadCoding(IFormFile file);
    }
}

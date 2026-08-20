using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Interface
{
    public interface IStudentRegRepository
    {
      Task<List<StudentRegisterResponse>> RegisterBulkStudentAsync(IFormFile file);

    }
}

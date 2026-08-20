using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Interface
{
    public interface ICollegeRegRepository
    {
      Task<CollegeRegistrationResponse> RegisterCollegeAsync(RegisterCollegeRequest request);
       
    }
}

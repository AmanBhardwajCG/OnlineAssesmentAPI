using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Interface
{
    public interface IGetDropDownRepo
    {
        Task<List<GetCollegebyNameID>> GetDropDownAsync();
    }
}

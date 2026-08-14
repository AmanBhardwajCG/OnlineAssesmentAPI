using OnlineAssesmentAPI.Class;
using OnlineAssesmentAPI.Repositories;

namespace OnlineAssesmentAPI.Interface
{
    public interface IUsersRepository
    {
        Task<long> CreateUsersAsync(Users user);
    }
}

using OnlineAssesmentAPI.Class;
using OnlineAssesmentAPI.Repositories;
using static OnlineAssesmentAPI.ModelClass.UserLogin;

namespace OnlineAssesmentAPI.Interface
{
    public interface IUsersRepository
    {
        Task<long> CreateUsersAsync(Users user);
        Task<LoginResponse?> LoginAsync(UserLoginRequest request);
    }
}

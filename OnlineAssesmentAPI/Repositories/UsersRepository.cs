using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Class;
using OnlineAssesmentAPI.Data;
using OnlineAssesmentAPI.Interface;
using System.Data;
using System.Xml.Linq;
using static OnlineAssesmentAPI.Class.AppEnum;
using static OnlineAssesmentAPI.ModelClass.UserLogin;

namespace OnlineAssesmentAPI.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UsersRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<long> CreateUsersAsync(Users user)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("sp_user_create", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            //command.Parameters.AddWithValue("@RoleId", (int)user.Role);
            if (!Enum.TryParse<Role>(user.Role, true, out var role))
            {
                throw new ArgumentException("Invalid role");
            }

            command.Parameters.AddWithValue("@RoleId", (int)role);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);

        }

        public async Task<LoginResponse?> LoginAsync(UserLoginRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_user_login", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = request.Email;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            string dbPasswordHash = reader["PasswordHash"].ToString();
            bool IsActive = Convert.ToBoolean(reader["IsActive"]);
            bool validPassword = BCrypt.Net.BCrypt.Verify(
              request.Password,
             dbPasswordHash
               );
            if (!validPassword)
                return null;
            // Temporary plain-text comparison
            //if (dbPassword != request.Password)
            //    return null;

            // Check inactive account
            if (!IsActive)
            {
                return new LoginResponse
                {
                    IsActive = false,
                    Message = "Your account is inactive. Please contact the administrator."
                };
            }
            return new LoginResponse
            {
                Message="Login successfully",
                Userid = Convert.ToInt64(reader["UserId"]),
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                RoleName = reader["RoleName"].ToString(),
                IsActive = IsActive
            };
        }

        public async Task<List<GetUsers>> GetUsers()
        {
            using var command = new SqlCommand("Usp_GetAllUser", _connectionFactory.CreateConnection());
            command.CommandType = CommandType.StoredProcedure;

            await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            var users = new List<GetUsers>();

            while (await reader.ReadAsync())
            {
                users.Add(new GetUsers
                {
                    Userid = Convert.ToInt64(reader["UserId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    RoleName = reader["RoleName"].ToString()
                });
            }

            return users;
        }

    }
}

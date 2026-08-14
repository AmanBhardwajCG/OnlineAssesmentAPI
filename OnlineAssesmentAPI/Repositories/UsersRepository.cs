using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Class;
using OnlineAssesmentAPI.Data;
using OnlineAssesmentAPI.Interface;
using System.Data;
using System.Xml.Linq;
using static OnlineAssesmentAPI.Class.AppEnum;

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
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
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
     
    }
}

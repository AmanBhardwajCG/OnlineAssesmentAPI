using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Data;
using static OnlineAssesmentAPI.Repositories.CollegeRegRepository;

namespace OnlineAssesmentAPI.Repositories
{
        public class CollegeRegRepository : ICollegeRegRepository
        {
            private readonly IConfiguration _configuration;

            public CollegeRegRepository(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<CollegeRegistrationResponse> RegisterCollegeAsync(
                RegisterCollegeRequest request)
            {
                var response = new CollegeRegistrationResponse();

                string? connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                using SqlConnection connection =
                    new SqlConnection(connectionString);

                using SqlCommand command =
                    new SqlCommand("sp_College_Register", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue(
                    "@CollegeName",
                    request.CollegeName);

                await connection.OpenAsync();

                using SqlDataReader reader =
                    await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    response.IsSuccess =
                        reader.GetBoolean(
                            reader.GetOrdinal("IsSuccess"));

                    response.Message =
                        reader.GetString(
                            reader.GetOrdinal("Message"));

                    if (!reader.IsDBNull(
                        reader.GetOrdinal("CollegeId")))
                    {
                        response.CollegeId =
                            reader.GetInt32(
                                reader.GetOrdinal("CollegeId"));
                    }
                }

                return response;
            }
        }
}


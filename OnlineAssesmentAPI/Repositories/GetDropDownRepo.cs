using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Data;

namespace OnlineAssesmentAPI.Repositories
{
    public class GetDropDownRepo :IGetDropDownRepo
    {
        private readonly IConfiguration _configuration;
        public GetDropDownRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<GetCollegebyNameID>> GetDropDownAsync()
        {
            var dropDownValues = new List<GetCollegebyNameID>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var query = "Select * from Colleges";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dropDownValues.Add(new GetCollegebyNameID
                            {
                                CollegeID = reader.GetInt32("CollegeID"),
                                CollegeName = reader.GetString("CollegeName")
                            });
                        }
                    }
                }
            }
            return dropDownValues;
        }
    }
}

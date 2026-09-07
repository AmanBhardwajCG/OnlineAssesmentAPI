using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass.ExamModel;
using System.Data;

namespace OnlineAssesmentAPI.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly IConfiguration _configuration;
        public ExamRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }   

        public async Task<ExamResponse> CreateExamAsync(ExamResponse request, long createdByUserId)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await using var command = new SqlCommand("USP_CreateExam", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add("@ExamName", SqlDbType.NVarChar, 200)
           .Value = request.ExamName;

            command.Parameters.Add("@Description", SqlDbType.NVarChar)
                .Value = (object?)request.Description ?? DBNull.Value;

            command.Parameters.Add("@DurationMinutes", SqlDbType.Int)
                .Value = request.DurationMinutes;

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 30)
    .Value = request.Status ?? "Active";

            command.Parameters.Add("@CreatedByUserId", SqlDbType.BigInt)
                .Value = createdByUserId;

            command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2)
                .Value = DateTime.UtcNow;

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {

                return new ExamResponse
                {
                    ExamId = Convert.ToInt32(reader["ExamId"]),
                    ExamName = reader["ExamName"].ToString()!,
                    Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),

                    DurationMinutes = Convert.ToInt32(reader["DurationMinutes"]),

                    Status = reader["Status"].ToString()!,

                    CreatedByUserId = Convert.ToInt64(createdByUserId),

                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])

                };
            }
            throw new Exception("Exam could not be created.");

         }

        public async Task<bool> PublishExamAsync(ExamReview Review)
        {
            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await using SqlCommand command = new SqlCommand("usp_Exam_Publish_Archive", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ExamId",
                Review.ExamId);

            command.Parameters.AddWithValue(
                "@Status",
                Review.Status);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToBoolean(result);
        }

        public async Task<(bool IsSuccess, string Message)> AssignCollegeAsync(AssignExamCollegeRequest request)
        {
            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await using SqlCommand command =
                new SqlCommand("usp_Exam_AssignCollege", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ExamId",
                request.ExamId);

            command.Parameters.AddWithValue(
                "@CollegeId",
                request.CollegeId);

            command.Parameters.AddWithValue(
                "@StartAt",
                request.StartAt);

            command.Parameters.AddWithValue(
                "@EndAt",
                request.EndAt);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                bool isSuccess = Convert.ToBoolean(
                    reader["IsSuccess"]);

                string message = Convert.ToString(
                    reader["Message"]) ?? string.Empty;

                return (isSuccess, message);
            }

            return (false, "Unable to assign exam to college.");
        }

      
        public async Task<StudentExamDTO> GetStudentExamsAsync(string email, string rollnumber)
        {
            var List = new List<StudentExamDTO>();
            var Response = new StudentExamDTO();
            await using SqlConnection connection =
                new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await using SqlCommand command = new SqlCommand("sp_Student_GetExams", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@Email",
                email);

            command.Parameters.AddWithValue(
                "@RollNumber",
                rollnumber);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            DataTable table = new DataTable();
            while (await reader.ReadAsync())
            {
                //student

                if (Response.Student == null)
                {
                    Response.Student = new StudentResponse
                    {
                        StudentId = Convert.ToInt64(
                        reader["StudentId"]),

                        RollNumber = reader["RollNumber"]?.ToString()
                             ?? string.Empty,

                        Name = reader["StudentName"]?.ToString()
                           ?? string.Empty,

                        Batch = reader["Batch"]?.ToString()
                            ?? string.Empty,

                        Course = reader["Course"]?.ToString()
                             ?? string.Empty,

                        Email = reader["Email"]?.ToString()
                            ?? string.Empty,



                        CollegeId = Convert.ToInt32(
                        reader["CollegeId"])
                    };
                }
                Response.Exam.Add(new StudentExamResponse
                {

                    ExamId = Convert.ToInt32(reader["ExamId"]),
                    ExamName = reader["ExamName"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    DurationMinutes = Convert.ToInt32(reader["DurationMinutes"]),
                    TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                    MCQCount = Convert.ToInt32(reader["MCQCount"]),
                    CodingCount = Convert.ToInt32(reader["CodingCount"]),
                    StartAt = Convert.ToDateTime(reader["StartAt"]),
                    EndAt = Convert.ToDateTime(reader["EndAt"])
                });
                     

                }

                //List.Add(Response);

            

                return Response;

        }
        public async Task<List<EnrollStudentResponse>> GetEligibleStudentsAsync(long examId)
        {
            var list = new List<EnrollStudentResponse>();
            await using SqlConnection connection =
                new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await using SqlCommand command =
                new SqlCommand("sp_Exam_GetEnrollStudents", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ExamId",
                examId);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            DataTable table = new DataTable();

            while (await reader.ReadAsync())
            {
                var student = new EnrollStudentResponse
                {
                    StudentId = Convert.ToInt64(
                        reader["StudentId"]),

                    RollNo = reader["RollNumber"]?.ToString()
                             ?? string.Empty,

                    Name = reader["StudentName"]?.ToString()
                           ?? string.Empty,

                    Batch = reader["Batch"]?.ToString()
                            ?? string.Empty,

                    Course = reader["Course"]?.ToString()
                             ?? string.Empty,

                    Email = reader["Email"]?.ToString()
                            ?? string.Empty,

                    MobileNo = reader["MobileNo"]?.ToString()
                               ?? string.Empty,

                    CollegeId = Convert.ToInt32(
                        reader["CollegeId"])
                };

                list.Add(student);
            }

            return list;


        }


    }
}


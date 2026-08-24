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

        public async Task<long> CreateExamAsync(CreateExamRequest request, long createdByUserId)
        {
            await using SqlConnection connection =
            new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await using SqlCommand command =
                new SqlCommand("sp_Exam_Create", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ExamName", request.ExamName);

            command.Parameters.AddWithValue(
                "@Description",
                (object?)request.Description ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@DurationMinutes",
                request.DurationMinutes);

            command.Parameters.AddWithValue(
                "@TotalQuestions",
                request.TotalQuestions);

            command.Parameters.AddWithValue(
                "@MCQCount",
                request.MCQCount);

            command.Parameters.AddWithValue(
                "@CodingCount",
                request.CodingCount);

            //command.Parameters.AddWithValue(
            //    "@StartAt",
            //    request.StartAt);

            //command.Parameters.AddWithValue(
            //    "@EndAt",
            //    request.EndAt);

            command.Parameters.AddWithValue(
                "@CreatedByUserId",
                 createdByUserId);

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt64(result);


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

      
        public async Task<List<StudentExamResponse>> GetStudentExamsAsync(string email, string rollnumber)
        {
            var List = new List<StudentExamResponse>();
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
                var exam = new StudentExamResponse
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

                    ExamId = Convert.ToInt32(reader["ExamId"]),

                    CollegeId = Convert.ToInt32(
                        reader["CollegeId"]),

                    ExamName = reader["ExamName"]?.ToString()?? string.Empty,
                    Description = reader["Description"]?.ToString()?? string.Empty,
                   DurationMinutes= Convert.ToInt32(reader["DurationMinutes"]),
                   TotalQuestions= Convert.ToInt32(reader["TotalQuestions"]),
                   MCQCount= Convert.ToInt32(reader["MCQCount"]),
                    CodingCount = Convert.ToInt32(reader["CodingCount"]),
                   // status = reader["Status"]?.ToString()?? string.Empty,
                    StartAt = Convert.ToDateTime(reader["StartAt"]),
                    EndAt = Convert.ToDateTime(reader["EndAt"])
                     

                };

                List.Add(exam);

            }

                return List;

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


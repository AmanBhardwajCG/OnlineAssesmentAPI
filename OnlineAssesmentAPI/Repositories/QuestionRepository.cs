using ClosedXML.Excel;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Data;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using ServiceStack.Text;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.NetworkInformation;

namespace OnlineAssesmentAPI.Repositories
{
    public class QuestionRepository: IQuestionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public QuestionRepository(
            DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Question>> GetAllMcqQuestions()
        {
            var questions = new List<Question>();

            using var connection = _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var cmd = new SqlCommand("USP_GetAllMcqQuestions",
                (SqlConnection)connection);

            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                questions.Add(new Question
                {
                    QuestionId = Convert.ToInt32(reader["QuestionId"]),
                    QuestionText = reader["QuestionText"]?.ToString(),
                    QuestionType = reader["QuestionType"]?.ToString(),
                    Difficulty = reader["Difficulty"]?.ToString(),
                    CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"]),
                    Status = reader["Status"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["UpdatedAt"])
                });
            }

            return questions;
        }

        //---------Upload MCQ Questions from CSV or Excel file to Database using Stored Procedure----------------------------
        public async Task<string> UploadMcq(IFormFile file)
        {
            var questions = new List<Question>();

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                var csv = new CsvHelper.CsvReader(reader, config);

                var records = csv.GetRecords<Question>()
                                 .Where(x => !string.IsNullOrWhiteSpace(x.QuestionText)).ToList();

                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                foreach (var item in records)
                {
                    using var cmd = new SqlCommand("USP_InsertMultipleChoiceQuestion", connection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QuestionText", item.QuestionText ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuestionType", "MCQ");
                    cmd.Parameters.AddWithValue("@Difficulty", item.Difficulty ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedByUserId", 1);
                    cmd.Parameters.AddWithValue("@Status", true);
                    cmd.Parameters.AddWithValue("@Option1", item.Option1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option2", item.Option2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option3", item.Option3 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option4", item.Option4 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CorrectAnswer", item.CorrectAnswer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Topic", item.Topic ?? (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }

                return $"{records.Count} MCQ questions uploaded successfully from .csv file.";
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);

                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);

                await using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                foreach (var row in rows)
                {
                    var question = new Question
                    {
                        QuestionText = row.Cell(1).GetValue<string>(),
                        Difficulty = row.Cell(2).GetValue<string>(),
                        QuestionType = "MCQ",
                        Status = "1",
                        CreatedByUserId = 1,
                        CreatedAt = DateTime.UtcNow,
                        Option1 = row.Cell(3).GetValue<string>(),
                        Option2 = row.Cell(4).GetValue<string>(),
                        Option3 = row.Cell(5).GetValue<string>(),
                        Option4 = row.Cell(6).GetValue<string>(),
                        CorrectAnswer = row.Cell(7).GetValue<string>()
                    };

                    using var cmd = new SqlCommand("USP_InsertMultipleChoiceQuestion", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                    cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                    cmd.Parameters.AddWithValue("@Difficulty", question.Difficulty ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedByUserId", question.CreatedByUserId);
                    cmd.Parameters.AddWithValue("@Status", question.Status);
                    cmd.Parameters.AddWithValue("@Option1", question.Option1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option2", question.Option2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option3", question.Option3 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Option4", question.Option4 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Topic", question.Topic ?? (object)DBNull.Value);
                    var result = await cmd.ExecuteNonQueryAsync();

                }
                return $"MCQ questions uploaded successfully from .xlsx file.";
            }
            else
            {
                throw new Exception("Only .csv and .xlsx files are supported.");
            }
        }

        //--------Upload Coding Questions from CSV or Excel file to Database using Stored Procedure----------------------------
        public async Task<string> UploadCoding(IFormFile file)
        {
            var questions = new List<CodingQuestion>();

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                var csv = new CsvHelper.CsvReader(reader, config);

                var records = csv.GetRecords<CodingQuestion>()
                                 .Where(x => !string.IsNullOrWhiteSpace(x.ProblemStatement)).ToList();

                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                foreach (var item in records)
                {
                    using var cmd = new SqlCommand("USP_InsertCodingQuestions", connection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProblemStatement", item.ProblemStatement ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InputDescription", item.InputDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OutputDescription", item.OutputDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FunctionParameterType", item.FunctionParameterType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FunctionReturnType", item.FunctionReturnType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Constraints", item.Constraints ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StarterCode", item.StarterCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Difficulty", item.Difficulty ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedBy", item.UpdatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", true);
                    cmd.Parameters.AddWithValue("@IsActive", item.IsActive);
                    //cmd.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);
                    //cmd.Parameters.AddWithValue("@UpdatedDate", item.UpdatedDate ?? (object)DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }

                return $"{records.Count} Coding questions uploaded successfully from .csv file.";
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);

                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);

                await using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                foreach (var row in rows)
                {
                    var question = new CodingQuestion
                    {
                        ProblemStatement = row.Cell(1).GetValue<string>(),
                        InputDescription = row.Cell(2).GetValue<string>(),
                        OutputDescription = row.Cell(3).GetValue<string>(),
                        Constraints = row.Cell(4).GetValue<string>(),
                        FunctionParameterType = row.Cell(5).GetValue<string>(),
                        FunctionReturnType = row.Cell(6).GetValue<string>(),
                        StarterCode = null,
                        Difficulty = row.Cell(7).GetValue<string>(),
                        CreatedBy = "1",
                        UpdatedBy = "1",
                        Status = "true",
                        IsActive = true,
                        //CreatedDate = DateTime.UtcNow,
                        //UpdatedDate = null
                    };

                    using var cmd = new SqlCommand("USP_InsertCodingQuestions", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProblemStatement", question.ProblemStatement ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InputDescription", question.InputDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OutputDescription", question.OutputDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FunctionParameterType", question.FunctionParameterType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FunctionReturnType", question.FunctionReturnType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Constraints", question.Constraints ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StarterCode", question.StarterCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Difficulty", question.Difficulty ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", question.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedBy", question.UpdatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", question.Status);
                    cmd.Parameters.AddWithValue("@IsActive", question.IsActive);
                    //cmd.Parameters.AddWithValue("@CreatedDate", question.CreatedDate);
                    //cmd.Parameters.AddWithValue("@UpdatedDate", question.UpdatedDate ?? (object)DBNull.Value);

                    var result = await cmd.ExecuteNonQueryAsync();
                }
                // await connection.CloseAsync();
                return $"Coding questions uploaded successfully from .xlsx file.";
            }
            else
            {
                throw new Exception("Only .csv and .xlsx files are supported.");
            }
        }

        #region CreateManualQues comment
        //public async Task<long> CreateQuestionAsync( Question request,
        //        long createdByUserId)
        //    {
        //        using var connection =
        //            _connectionFactory.CreateConnection();

        //        await connection.OpenAsync();

        //        using var transaction =
        //            await connection.BeginTransactionAsync();

        //        try
        //        {
        //            // 1. Create Question
        //            using var questionCommand =
        //                new SqlCommand(
        //                    "sp_Question_Create",
        //                    connection,
        //                    (SqlTransaction)transaction);

        //            questionCommand.CommandType =
        //                CommandType.StoredProcedure;

        //            questionCommand.Parameters.Add(
        //                "@QuestionText",
        //                SqlDbType.NVarChar).Value =
        //                request.QuestionText;

        //            questionCommand.Parameters.Add(
        //                "@QuestionType",
        //                SqlDbType.NVarChar, 20).Value =
        //                request.QuestionType;

        //            questionCommand.Parameters.Add(
        //                "@Difficulty",
        //                SqlDbType.NVarChar, 30).Value =
        //                (object?)request.Difficulty ??
        //                DBNull.Value;

        //            questionCommand.Parameters.Add(
        //                "@CreatedByUserId",
        //                SqlDbType.BigInt).Value =
        //                createdByUserId;

        //            var questionId =
        //                Convert.ToInt64(
        //                    await questionCommand.ExecuteScalarAsync());


        //            // 2. Create Options
        //            foreach (var option in request.Options)
        //            {
        //                using var optionCommand =
        //                    new SqlCommand(
        //                        "sp_QuestionOption_Create",
        //                        connection,
        //                        (SqlTransaction)transaction);

        //                optionCommand.CommandType =
        //                    CommandType.StoredProcedure;

        //                optionCommand.Parameters.Add(
        //                    "@QuestionId",
        //                    SqlDbType.BigInt).Value =
        //                    questionId;

        //                optionCommand.Parameters.Add(
        //                    "@OptionText",
        //                    SqlDbType.NVarChar, 500).Value =
        //                    option.OptionText;

        //                optionCommand.Parameters.Add(
        //                    "@IsCorrect",
        //                    SqlDbType.Bit).Value =
        //                    option.IsCorrect;

        //                await optionCommand.ExecuteScalarAsync();
        //            }

        //            await transaction.CommitAsync();

        //            return questionId;
        //        }
        //        catch
        //        {
        //            await transaction.RollbackAsync();
        //            throw;
        //        }
        //    }
        #endregion


        //public async Task<bool> PublishQuestionAsync(QuestionReview Review)
        //{
        //    try
        //    {
        //        using var connection = _connectionFactory.CreateConnection();
        //        using var command = new SqlCommand("sp_Question_Publish_Archive", connection);

        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@QuestionId", SqlDbType.BigInt).Value = Review.QuestionId;
        //        command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = Review.Status;

        //        await connection.OpenAsync();
        //        object? result = await command.ExecuteScalarAsync();
        //        return result != null && Convert.ToBoolean(result);
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //public async Task<MCQQuestion> GetMCQQuestionAsync()
        //{

        //}
    }
}


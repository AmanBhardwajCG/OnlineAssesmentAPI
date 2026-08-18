using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OnlineAssesmentAPI.Data;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;

namespace OnlineAssesmentAPI.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly DbConnectionFactory _context;

        public QuestionRepository(DbConnectionFactory context)
        {
            _context = context;
        }

        //---------Upload MCQ Questions from CSV or Excel file to Database using Stored Procedure----------------------------
        public async Task<string> UploadMcq(IFormFile file)
        {
            var questions = new List<Questions>();

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<Questions>()
                                 .Where(x=> !string.IsNullOrWhiteSpace(x.QuestionText)).ToList();

                using var connection = _context.CreateConnection();
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

                await using var connection = _context.CreateConnection();
                await connection.OpenAsync();

                foreach (var row in rows)
                {
                    var question = new Questions
                    {
                        QuestionText = row.Cell(1).GetValue<string>(),
                        Difficulty = row.Cell(2).GetValue<string>(),
                        QuestionType = "MCQ",
                        Status = true,
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

                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<CodingQuestion>()
                                 .Where(x => !string.IsNullOrWhiteSpace(x.ProblemStatement)).ToList();

                using var connection = _context.CreateConnection();
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

                await using var connection = _context.CreateConnection();
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
                        IsActive= true,
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
    }
}

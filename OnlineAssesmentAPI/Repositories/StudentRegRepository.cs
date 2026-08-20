using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using ExcelDataReader;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OnlineAssesmentAPI.Repositories
{
    public class StudentRegRepository : IStudentRegRepository
    {
        private readonly IConfiguration _configuration;
        public StudentRegRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #region comment
        //public async Task<StudentRegisterResponse> RegisterStudentAsync(StudentExcelRow student)
        //{
        //    var response = new StudentRegisterResponse
        //    {
        //        RowNumber = student.RowNumber,
        //        RollNo = student.RollNo
        //    };

        //    try
        //    {
        //        var connectionString =
        //            _configuration.GetConnectionString("DefaultConnection");

        //        await using var connection =
        //            new SqlConnection(connectionString);

        //        await using var command =
        //            new SqlCommand(
        //                "sp_Student_BulkRegister",
        //                connection);

        //        command.CommandType =
        //            CommandType.StoredProcedure;

        //        command.Parameters.Add(
        //            "@CollegeName",
        //            SqlDbType.NVarChar, 200)
        //            .Value = student.CollegeName;

        //        command.Parameters.Add(
        //            "@RollNo",
        //            SqlDbType.NVarChar, 50)
        //            .Value = student.RollNo;

        //        command.Parameters.Add(
        //            "@Name",
        //            SqlDbType.NVarChar, 150)
        //            .Value = student.Name;

        //        command.Parameters.Add(
        //            "@Batch",
        //            SqlDbType.NVarChar, 50)
        //            .Value = student.Batch;

        //        command.Parameters.Add(
        //            "@Course",
        //            SqlDbType.NVarChar, 150)
        //            .Value = student.Course;

        //        command.Parameters.Add(
        //            "@Email",
        //            SqlDbType.NVarChar, 200)
        //            .Value = student.Email;

        //        command.Parameters.Add(
        //            "@MobileNo",
        //            SqlDbType.NVarChar, 20)
        //            .Value = student.MobileNo;

        //        await connection.OpenAsync();

        //        await using var reader =
        //            await command.ExecuteReaderAsync();

        //        if (await reader.ReadAsync())
        //        {
        //            response.IsSuccess =
        //                Convert.ToBoolean(reader["IsSuccess"]);

        //            response.Message =
        //                reader["Message"]?.ToString()
        //                ?? string.Empty;

        //            if (reader["StudentId"] != DBNull.Value)
        //            {
        //                response.StudentId =
        //                    Convert.ToInt64(reader["StudentId"]);
        //            }
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = ex.Message;

        //        return response;
        //    }
        //}
        #endregion

        //public async Task<List<StudentRegisterResponse>> RegisterBulkStudentAsync(IFormFile file)
        //{
        //    var results =
        //        new List<StudentRegisterResponse>();

        //    // -----------------------------
        //    // File validation
        //    // -----------------------------

        //    if (file == null || file.Length == 0)
        //    {
        //        throw new ArgumentException(
        //            "Excel file is required.");
        //    }

        //    var extension =
        //        Path.GetExtension(file.FileName)
        //            .ToLowerInvariant();

        //    if (extension != ".xlsx" &&
        //        extension != ".xls")
        //    {
        //        throw new ArgumentException(
        //            "Only .xlsx or .xls files are allowed.");
        //    }

        //    // -----------------------------
        //    // Read Excel
        //    // -----------------------------

        //    using var stream =file.OpenReadStream();

        //    using var reader =
        //        ExcelReaderFactory.CreateReader(stream);

        //    var isHeader = true;

        //    var rowNumber = 0;

        //    while (reader.Read())
        //    {
        //        rowNumber++;

        //        // Skip header
        //        if (isHeader)
        //        {
        //            isHeader = false;
        //            continue;
        //        }

        //        // Skip empty row
        //        if (IsEmptyRow(reader))
        //        {
        //            continue;
        //        }

        //        // -----------------------------
        //        // Read Excel columns
        //        // -----------------------------

        //        var collegeName =
        //            reader.GetValue(0)?
        //                .ToString()?.Trim() ?? "";

        //        var rollNo =
        //            reader.GetValue(1)?
        //                .ToString()?.Trim() ?? "";

        //        var name =
        //            reader.GetValue(2)?
        //                .ToString()?.Trim() ?? "";

        //        var batch =
        //            reader.GetValue(3)?
        //                .ToString()?.Trim() ?? "";

        //        var course =
        //            reader.GetValue(4)?
        //                .ToString()?.Trim() ?? "";

        //        var email =
        //            reader.GetValue(5)?
        //                .ToString()?.Trim() ?? "";

        //        var mobileNo =
        //            reader.GetValue(6)?
        //                .ToString()?.Trim() ?? "";

        //        // -----------------------------
        //        // Validate Excel row
        //        // -----------------------------

        //        var validationError =
        //            ValidateRow(
        //                collegeName,
        //                rollNo,
        //                name,
        //                batch,
        //                course,
        //                email);

        //        if (validationError != null)
        //        {
        //            results.Add(new StudentRegisterResponse
        //            {
        //                RowNumber = rowNumber,
        //                RollNo = rollNo,
        //                IsSuccess = false,
        //                Message = validationError,
        //                StudentId = null
        //            });

        //            continue;
        //        }

        //        // -----------------------------
        //        // Call Stored Procedure
        //        // -----------------------------

        //        var result =
        //            await RegisterStudentAsync(
        //                rowNumber,
        //                collegeName,
        //                rollNo,
        //                name,
        //                batch,
        //                course,
        //                email,
        //                mobileNo);

        //        results.Add(result);
        //    }

        //    return results;
        //}


        public async Task<List<StudentRegisterResponse>> RegisterBulkStudentAsync(IFormFile file)
        {   
            var results = new List<StudentRegisterResponse>();

        var extension =
            Path.GetExtension(file.FileName)
                .ToLowerInvariant();

    if (extension != ".xlsx")
    {
        throw new Exception(
            "Only .xlsx files are supported.");
    }

    using var stream = file.OpenReadStream();

    using var workbook = new XLWorkbook(stream);

    var worksheet = workbook.Worksheet(1);

    var rows = worksheet.RowsUsed().Skip(1);

    //await using var connection =
    //    Context.CreateConnection();

    await using var connection =
        new SqlConnection(
            _configuration.GetConnectionString(
                "DefaultConnection"));

            await connection.OpenAsync();

    foreach (var row in rows)
    {
        var rowNumber = row.RowNumber();

    var collegeName =
        row.Cell(1).GetValue<string>().Trim();

    var rollNo =
        row.Cell(2).GetValue<string>().Trim();

    var name =
        row.Cell(3).GetValue<string>().Trim();

    var batch =
        row.Cell(4).GetValue<string>().Trim();

    var course =
        row.Cell(5).GetValue<string>().Trim();

    var email =
        row.Cell(6).GetValue<string>().Trim();

    var mobileNo =
        row.Cell(7).GetValue<string>().Trim();

        // Basic validation
        if (string.IsNullOrWhiteSpace(rollNo))
        {
            results.Add(new StudentRegisterResponse
            {
        RowNumber = rowNumber,
                RollNo = rollNo,
                IsSuccess = false,
                Message = "Roll number is required."
            });

            continue;
        }

if (string.IsNullOrWhiteSpace(name))
{
    results.Add(new StudentRegisterResponse
    {
        RowNumber = rowNumber,
        RollNo = rollNo,
        IsSuccess = false,
        Message = "Student name is required."
    });

    continue;
}

using var cmd =
    new SqlCommand(
        "sp_Student_BulkRegister",
        connection);

cmd.CommandType =
    CommandType.StoredProcedure;

cmd.Parameters.Add(
    "@CollegeName",
    SqlDbType.NVarChar, 200)
    .Value = collegeName;

cmd.Parameters.Add(
    "@RollNo",
    SqlDbType.NVarChar, 50)
    .Value = rollNo;

cmd.Parameters.Add(
    "@Name",
    SqlDbType.NVarChar, 150)
    .Value = name;

cmd.Parameters.Add(
    "@Batch",
    SqlDbType.NVarChar, 50)
    .Value = batch;

cmd.Parameters.Add(
    "@Course",
    SqlDbType.NVarChar, 150)
    .Value = course;

cmd.Parameters.Add(
    "@Email",
    SqlDbType.NVarChar, 200)
    .Value = email;

cmd.Parameters.Add(
    "@MobileNo",
    SqlDbType.NVarChar, 20)
    .Value =
        string.IsNullOrWhiteSpace(mobileNo)
            ? DBNull.Value
            : mobileNo;

await using var reader =
    await cmd.ExecuteReaderAsync();

if (await reader.ReadAsync())
{
    results.Add(new StudentRegisterResponse
    {
        RowNumber = rowNumber,

        RollNo = rollNo,

        IsSuccess =
            Convert.ToBoolean(
                reader["IsSuccess"]),

        Message =
            reader["Message"]?.ToString()
            ?? string.Empty,

        StudentId =
            reader["StudentId"] == DBNull.Value
                ? null
                : Convert.ToInt64(
                    reader["StudentId"])
    });
}
    }

    return results;
}

        private async Task<StudentRegisterResponse>
            RegisterStudentAsync(
                int rowNumber,
                string collegeName,
                string rollNo,
                string name,
                string batch,
                string course,
                string email,
                string mobileNo)
        {
            var result =
                new StudentRegisterResponse
                {
                    RowNumber = rowNumber,
                    RollNo = rollNo
                };

            var connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");

            await using var connection =
                new SqlConnection(connectionString);

            await using var command =
                new SqlCommand(
                    "sp_Student_BulkRegister",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            // CollegeName
            command.Parameters.Add(
                "@CollegeName",
                SqlDbType.NVarChar,
                200).Value =
                    collegeName;

            // RollNo
            command.Parameters.Add(
                "@RollNo",
                SqlDbType.NVarChar,
                50).Value =
                    rollNo;

            // Name
            command.Parameters.Add(
                "@Name",
                SqlDbType.NVarChar,
                150).Value =
                    name;

            // Batch
            command.Parameters.Add(
                "@Batch",
                SqlDbType.NVarChar,
                50).Value =
                    batch;

            // Course
            command.Parameters.Add(
                "@Course",
                SqlDbType.NVarChar,
                150).Value =
                    course;

            // Email
            command.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                200).Value =
                    email;

            // MobileNo
            command.Parameters.Add(
                "@MobileNo",
                SqlDbType.NVarChar,
                20).Value =
                    string.IsNullOrWhiteSpace(mobileNo)
                        ? DBNull.Value
                        : mobileNo;

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result.IsSuccess =
                    Convert.ToBoolean(
                        reader["IsSuccess"]);

                result.Message =
                    reader["Message"]?.ToString()
                    ?? string.Empty;

                if (reader["StudentId"] !=
                    DBNull.Value)
                {
                    result.StudentId =
                        Convert.ToInt64(
                            reader["StudentId"]);
                }
            }
            else
            {
                result.IsSuccess = false;

                result.Message =
                    "No response received from database.";
            }

            return result;
        }

        private static bool IsEmptyRow(
            IExcelDataReader reader)
        {
            for (var i = 0;
                 i < reader.FieldCount;
                 i++)
            {
                var value =
                    reader.GetValue(i)?
                        .ToString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }
            }

            return true;
        }

        private static string? ValidateRow(
            string collegeName,
            string rollNo,
            string name,
            string batch,
            string course,
            string email)
        {
            if (string.IsNullOrWhiteSpace(
                collegeName))
            {
                return "College name is required.";
            }

            if (string.IsNullOrWhiteSpace(
                rollNo))
            {
                return "Roll number is required.";
            }

            if (string.IsNullOrWhiteSpace(
                name))
            {
                return "Student name is required.";
            }

            if (string.IsNullOrWhiteSpace(
                batch))
            {
                return "Batch is required.";
            }

            if (string.IsNullOrWhiteSpace(
                course))
            {
                return "Course is required.";
            }

            if (string.IsNullOrWhiteSpace(
                email))
            {
                return "Email is required.";
            }

            //if (!IsValidEmail(email))
            //{
            //    return "Invalid email.";
            //}

            return null;
        }

        //private static bool IsValidEmail(
        //    string email)
        //{
        //    return Regex.IsMatch(
        //        email,
        //        @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        //}
    }
}
    




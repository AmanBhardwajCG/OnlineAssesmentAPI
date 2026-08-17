using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Data;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Data;

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
    
            public async Task<long> CreateQuestionAsync( Question request,
                long createdByUserId)
            {
                using var connection =
                    _connectionFactory.CreateConnection();

                await connection.OpenAsync();

                using var transaction =
                    await connection.BeginTransactionAsync();

                try
                {
                    // 1. Create Question
                    using var questionCommand =
                        new SqlCommand(
                            "sp_Question_Create",
                            connection,
                            (SqlTransaction)transaction);

                    questionCommand.CommandType =
                        CommandType.StoredProcedure;

                    questionCommand.Parameters.Add(
                        "@QuestionText",
                        SqlDbType.NVarChar).Value =
                        request.QuestionText;

                    questionCommand.Parameters.Add(
                        "@QuestionType",
                        SqlDbType.NVarChar, 20).Value =
                        request.QuestionType;

                    questionCommand.Parameters.Add(
                        "@Difficulty",
                        SqlDbType.NVarChar, 30).Value =
                        (object?)request.Difficulty ??
                        DBNull.Value;

                    questionCommand.Parameters.Add(
                        "@CreatedByUserId",
                        SqlDbType.BigInt).Value =
                        createdByUserId;

                    var questionId =
                        Convert.ToInt64(
                            await questionCommand.ExecuteScalarAsync());


                    // 2. Create Options
                    foreach (var option in request.Options)
                    {
                        using var optionCommand =
                            new SqlCommand(
                                "sp_QuestionOption_Create",
                                connection,
                                (SqlTransaction)transaction);

                        optionCommand.CommandType =
                            CommandType.StoredProcedure;

                        optionCommand.Parameters.Add(
                            "@QuestionId",
                            SqlDbType.BigInt).Value =
                            questionId;

                        optionCommand.Parameters.Add(
                            "@OptionText",
                            SqlDbType.NVarChar, 500).Value =
                            option.OptionText;

                        optionCommand.Parameters.Add(
                            "@IsCorrect",
                            SqlDbType.Bit).Value =
                            option.IsCorrect;

                        await optionCommand.ExecuteScalarAsync();
                    }

                    await transaction.CommitAsync();

                    return questionId;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
    }
}


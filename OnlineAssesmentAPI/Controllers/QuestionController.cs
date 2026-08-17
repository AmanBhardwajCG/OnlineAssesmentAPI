using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Security.Claims;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(
            IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost("CreateQuestion")]
        public async Task<IActionResult> CreateQuestion(
            CreateQuestionRequest request)
        {
            try
            {
                // Get logged-in user's ID from JWT
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                long userId =
                    Convert.ToInt64(userIdClaim.Value);

                var question = new Question
                {
                    QuestionText = request.QuestionText,
                    QuestionType = request.QuestionType,
                    Difficulty = request.Difficulty,
                    CreatedByUserId = userId,
                    Status = "Draft",
                    Options = request.Options
                };

                var questionId =
                    await _questionRepository
                        .CreateQuestionAsync(question, userId);

                return Created(
                    $"api/Question/{questionId}",
                    new
                    {
                        QuestionId = questionId,
                        Message = "Question created successfully"
                    });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the question.");
            }
        }


    }
}

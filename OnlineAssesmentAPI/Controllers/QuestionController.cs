using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using System.Security.Claims;

namespace OnlineAssesmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddQuestionsController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;

        public AddQuestionsController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        [HttpGet("mcq-list")]
        public async Task<IActionResult> GetAllMcqQuestions()
        {
            var result = await _questionRepository.GetAllMcqQuestions();
            return Ok(result);
        }
        [HttpPost("upload-mcq")]
        //[Route("api/[controller]")]
        public async Task<IActionResult> UploadMcq(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid file.");
            }

            var result = await _questionRepository.UploadMcq(file);

            return Ok(result);
        }

        [HttpPost("upload-coding")]
        public async Task<IActionResult> UploadCoding(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid file.");
            }

            var result = await _questionRepository.UploadCoding(file);

            return Ok(result);
        }
        //[Route("api/[controller]")]
        //[ApiController]
        //public class QuestionController : ControllerBase
        //{
        //    private readonly IQuestionRepository _questionRepository;

        //    public QuestionController(
        //        IQuestionRepository questionRepository)
        //    {
        //        _questionRepository = questionRepository;
        //    }

        //    [Authorize(Roles = "Admin,Faculty")]
        //    [HttpPost("CreateQuestion")]
        //    public async Task<IActionResult> CreateQuestion(
        //        CreateQuestionRequest request)
        //    {
        //        try
        //        {
        //            // Get logged-in user's ID from JWT
        //            var userIdClaim =
        //                User.FindFirst(ClaimTypes.NameIdentifier);

        //            if (userIdClaim == null)
        //            {
        //                return Unauthorized();
        //            }

        //            long userId =
        //                Convert.ToInt64(userIdClaim.Value);

        //            var question = new Question
        //            {
        //                QuestionText = request.QuestionText,
        //                QuestionType = request.QuestionType,
        //                Difficulty = request.Difficulty,
        //                CreatedByUserId = userId,
        //                Status = "Draft",
        //                Options = request.Options
        //            };

        //            var questionId =
        //                await _questionRepository
        //                    .CreateQuestionAsync(question, userId);

        //            return Created(
        //                $"api/Question/{questionId}",
        //                new
        //                {
        //                    QuestionId = questionId,
        //                    Message = "Question created successfully"
        //                });
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(
        //                StatusCodes.Status500InternalServerError,
        //                "An error occurred while creating the question.");
        //        }
        //    }

        //    //[Authorize(Roles ="Admin")]
        //    [AllowAnonymous]
        //    [HttpPost("QuestionReview")]
        //    public async Task<IActionResult> QuestionReview(QuestionReview request)
        //    {
        //        try
        //        {
        //            var result = await _questionRepository.PublishQuestionAsync(request);
        //            if (result)
        //            {
        //                return Ok(new { Message = "Question status updated successfully" });
        //            }
        //            else
        //            {
        //                return NotFound(new { Message = "Question not found...Not updated" });
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(
        //                StatusCodes.Status500InternalServerError,
        //                "An error occurred while updating the question status.");
        //        }
        //    }
    }
}

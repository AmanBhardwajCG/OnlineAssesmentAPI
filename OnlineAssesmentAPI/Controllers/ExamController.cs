using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass.ExamModel;
using System.Security.Claims;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository _examRepository;

        public ExamController(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        [HttpPost("CreateExam")]
        public async Task<IActionResult> CreateExam(CreateExamRequest request)
        {
            try
            {
            var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            long userId =
                Convert.ToInt64(userIdClaim.Value);
            var result = await _examRepository.CreateExamAsync(request, userId);
                if (result <= 0)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Exam creation failed."
                    });
                }

                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Exam created successfully.",
                    ExamId = result
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the question.");
            }
        }

        [HttpPost("AssignExamtoCollege")]
        public async Task<IActionResult> AssignExamToCollege(AssignExamCollegeRequest request)
        {
            try
            {
                var (isSuccess, message) = await _examRepository.AssignCollegeAsync(request);

                if (!isSuccess)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = message
                    });
                }

                return Ok(new
                {
                    IsSuccess = true,
                    Message = message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while assigning the exam to the college.");
            }
        }


        [HttpGet("GetEnrollStudentbyExamId")]
        public async Task<IActionResult> GetEnrollStudentbyExamId(long examId)
        {
            try
            {
                var table = await _examRepository.GetEligibleStudentsAsync(examId);
                return Ok(table);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching eligible students.");
            }
        }

        [HttpGet("GetExambyStudentEmail")]
        public async Task<IActionResult> GetExambyStudentEmail(string email,string rollNumber)
        {
            try
            {
                var exams = await _examRepository.GetStudentExamsAsync(email,rollNumber);
                return Ok(exams);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching exams for the student.");
            }
        }
    }
}

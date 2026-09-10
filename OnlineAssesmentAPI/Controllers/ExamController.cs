using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;
using OnlineAssesmentAPI.ModelClass.ExamModel;
using OnlineAssesmentAPI.Repositories;
using System.Security.Claims;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository _examRepository;
        private readonly IJwtService _jwtService;

        public ExamController(IExamRepository examRepository, IJwtService jwtService)
        {
            _examRepository = examRepository;
            _jwtService = jwtService;
        }

        [HttpPost("CreateExam")]
        public async Task<IActionResult> CreateExam([FromBody] ExamResponse createExamRequest)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                long userId = Convert.ToInt64(userIdClaim.Value);

                if (createExamRequest == null)
                {
                    return BadRequest("Exam Details are required !!");
                }
                if (string.IsNullOrWhiteSpace(createExamRequest.ExamName))
                {
                    return BadRequest("Exam name is required.");
                }

                if (createExamRequest.DurationMinutes <= 0)
                {
                    return BadRequest("Duration must be greater than 0.");
                }

                //if (createExamRequest.EndAt <= createExamRequest.StartAt)
                //{
                //    return BadRequest("End time must be greater than start time.");
                //}

                await _examRepository.CreateExamAsync(createExamRequest, userId);

                return Ok(new
                {
                    message = "Wohoo!! Exam created successfully."
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the question.");
            }
        }


        //[Authorize(Roles ="Admin")]
        [AllowAnonymous]
        [HttpPost("ExamReview")]
        public async Task<IActionResult> QuestionReview(ExamReview request)
        {
            try
            {
                var result = await _examRepository.PublishExamAsync(request);
                if (result)
                {
                    return Ok(new { Message = "Exam status updated successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Exam not found...Not updated" });
                }
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the exam status.");
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
                if(exams.Student == null)
                {
                    return Unauthorized(new
                    {
                        exams.Message
                    });
                }
                var token = _jwtService.GenerateStudentToken(exams.Student);
                return Ok(new {exams.Student,Exams = exams.Exam, Token = token });
                // return Ok(exams);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching exams for the student.");
            }
        }


        [HttpPost("StartExam/Resume")]
        public async Task<IActionResult> StartExam(long examId)
        {
            try
            {
                // Get StudentId from JWT
                long studentId = GetStudentIdFromToken();
                if (studentId == null)
                {
                    return Unauthorized();
                }
                var response =
                    await _examRepository.StartExamAttemptAsync(
                        examId,
                        studentId);

                if (!response.AttemptResponse.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch(UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while starting the exam.");
            }
        }


        private long GetStudentIdFromToken()
        {
            var studentIdClaim = User.FindFirst("studentId")?.Value;

            if (!long.TryParse(studentIdClaim, out long studentId))
            {
                throw new UnauthorizedAccessException(
                    "Student ID not found in token.");
            }

            return studentId;
        }

        [HttpGet("GetScheduledExamsForStudent")]
        public async Task<IActionResult> GetScheduledExamsForStudent(int studentId)
        {
            try
            {
                var exams = await _examRepository.GetScheduledExamsForStudentAsync(studentId);
                if (exams == null)
                {
                    return NotFound(new
                    {
                        Message = "No scheduled exams found for the student."
                    });
                }
                return Ok(exams.Exam);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching scheduled exams for the student.");
            }
        }
    }
}

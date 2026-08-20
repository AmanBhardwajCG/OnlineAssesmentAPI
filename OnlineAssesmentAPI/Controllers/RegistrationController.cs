using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.ModelClass;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
           private readonly ICollegeRegRepository _collegeRepository;
           private readonly IStudentRegRepository _studentRepository;


            public RegistrationController(ICollegeRegRepository collegeRepository, IStudentRegRepository studentRepository)
            {
                _collegeRepository = collegeRepository;
                _studentRepository = studentRepository;
            }

            [HttpPost("CollegeRegister")]
            public async Task<IActionResult> RegisterCollege(
                RegisterCollegeRequest request)
            {
                var result =
                    await _collegeRepository.RegisterCollegeAsync(request);

                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }


        

        [HttpPost("bulk-register")]
        public async Task<IActionResult> BulkRegister( IFormFile file)
        {
            try
            {
                var result =
                    await _studentRepository
                        .RegisterBulkStudentAsync(file);

                var successCount =
                    result.Count(x => x.IsSuccess);

                var failedCount =
                    result.Count(x => !x.IsSuccess);

                return Ok(new
                {
                    TotalRecords = result.Count,

                    SuccessCount = successCount,

                    FailedCount = failedCount,

                    Results = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
            //catch (Exception)
            //{
            //    return StatusCode(
            //        StatusCodes.Status500InternalServerError,
            //        new
            //        {
            //            Message =
            //                "Error occurred while importing students."
            //        });
            //}
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}
        
    

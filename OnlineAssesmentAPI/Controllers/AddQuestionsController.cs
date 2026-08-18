using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;

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
        }
}


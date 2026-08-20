using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetController : ControllerBase
    {
        private readonly IGetDropDownRepo _getDropDownRepo;
        public GetController(IGetDropDownRepo getDropDownRepo)
        {
            _getDropDownRepo = getDropDownRepo;
        }

        [HttpGet("GetRegisterCollege")]
        public async Task<IActionResult> GetRegisterCollege()
        {
            var dropDownValues = await _getDropDownRepo.GetDropDownAsync();
            return Ok(dropDownValues);
        }
    }
}

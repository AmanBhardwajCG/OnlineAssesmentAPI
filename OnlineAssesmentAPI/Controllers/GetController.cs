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

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _getDropDownRepo.GetRoles();
            return Ok(roles);
        }

    }
}

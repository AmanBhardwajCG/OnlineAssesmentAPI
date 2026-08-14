using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.Class;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Users user)
        {
            if (user == null)
                return BadRequest("User payload is required.");

            try
            {
                var id = await _usersRepository.CreateUsersAsync(user);
                // Return 201 Created with location header
                return Created($"api/Users/{id}", new { Id = id });
            }
            catch (Exception ex)
            {
                // Log exception if logging is available; return generic server error
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OnlineAssesmentAPI.Class;
using OnlineAssesmentAPI.Interface;
using OnlineAssesmentAPI.Repositories;
using OnlineAssesmentAPI.Services;
using static OnlineAssesmentAPI.ModelClass.UserLogin;

namespace OnlineAssesmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtService _jwtService;

        public UsersController(IUsersRepository usersRepository, IJwtService jwtService)
        {
            _usersRepository = usersRepository;
            _jwtService = jwtService;
        }

        // POST: api/Users
        //[Authorize(Roles = "Admin")]
        [HttpPost("CreateUser")]
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
             
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(new
                {
                    Message = "Email already exists." + user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var result = await _usersRepository.LoginAsync(request);
            if (!result.IsActive)
            {
                return Unauthorized(new
                {
                   result.Message
                });
            }
            if (result == null)
                return Unauthorized("Invalid email or password.");
            var token = _jwtService.GenerateToken(
        result.Userid,
        result.Name,
        result.RoleName
    );

            result.Token = token;
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssigmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServiceUser _serviceUser;

        public UserController(IServiceUser serviceUser)
        {
            _serviceUser = serviceUser;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ModelUser user)
        {
            var result = await _serviceUser.CreateUserAsync(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ModelUser user)
        {
            var result = await _serviceUser.LoginAsync(user.Email, user.Password);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _serviceUser.GetUserByEmailAsync(email);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result.Message);
        }
    }
}

using FileManager.DTO;
using FileManager.Models;
using FileManager.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet("{userId:int}")]
        public ValueTask<User> GetUser(int userId, CancellationToken cancellationToken) =>
            _userService.GetUserById(userId, cancellationToken);

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser(User user, CancellationToken cancellationToken)
        {
            await _userService.CreateUser(user, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { userId = user.UserId }, user);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public ValueTask<UserAuthDto> Authenticate(AuthenticateModel authenticateModel, CancellationToken cancellationToken) =>
            _userService.Authenticate(authenticateModel.Username, authenticateModel.Password, cancellationToken);
    }
}

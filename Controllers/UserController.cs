using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static List<User> Users = new List<User>();
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<User>>> GetAllUsers()
        {
            _logger.LogInformation("Retrieving all users");
            var users = Users.Where(u => u.IsActive).ToList();
            return Ok(ApiResponse<IEnumerable<User>>.CreateSuccess(users));
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<User>> GetUserById(int id)
        {
            _logger.LogInformation($"Retrieving user with ID: {id}");
            var user = Users.FirstOrDefault(u => u.Id == id && u.IsActive);
            if (user == null)
                return NotFound(ApiResponse<User>.CreateError("User not found"));
            
            return Ok(ApiResponse<User>.CreateSuccess(user));
        }

        [HttpPost]
        public ActionResult<ApiResponse<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<User>.CreateError("Invalid user data"));

            // Check for duplicate email/username
            if (Users.Any(u => u.Email == user.Email))
                return BadRequest(ApiResponse<User>.CreateError("Email already exists"));

            if (Users.Any(u => u.Username == user.Username))
                return BadRequest(ApiResponse<User>.CreateError("Username already exists"));

            // Hash password
            user.Password = HashPassword(user.Password);
            user.Id = Users.Count == 0 ? 1 : Users.Max(u => u.Id) + 1;
            Users.Add(user);

            _logger.LogInformation($"Created new user with ID: {user.Id}");
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                ApiResponse<User>.CreateSuccess(user, "User created successfully")
            );
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<User>> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<User>.CreateError("Invalid user data"));

            var existingUser = Users.FirstOrDefault(u => u.Id == id && u.IsActive);
            if (existingUser == null)
                return NotFound(ApiResponse<User>.CreateError("User not found"));

            // Check for duplicate email/username (excluding current user)
            if (Users.Any(u => u.Email == updatedUser.Email && u.Id != id))
                return BadRequest(ApiResponse<User>.CreateError("Email already exists"));

            if (Users.Any(u => u.Username == updatedUser.Username && u.Id != id))
                return BadRequest(ApiResponse<User>.CreateError("Username already exists"));

            existingUser.Username = updatedUser.Username;
            existingUser.Email = updatedUser.Email;
            if (!string.IsNullOrEmpty(updatedUser.Password))
                existingUser.Password = HashPassword(updatedUser.Password);

            _logger.LogInformation($"Updated user with ID: {id}");
            return Ok(ApiResponse<User>.CreateSuccess(existingUser, "User updated successfully"));
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<object>> DeleteUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id && u.IsActive);
            if (user == null)
                return NotFound(ApiResponse<object>.CreateError("User not found"));

            // Soft delete
            user.IsActive = false;
            _logger.LogInformation($"Soft deleted user with ID: {id}");
            return Ok(ApiResponse<object>.CreateSuccess(null, "User deleted successfully"));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
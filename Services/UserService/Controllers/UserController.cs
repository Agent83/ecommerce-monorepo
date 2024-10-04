using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserService.EventsBus;
using UserService.EventsBus.Model;
using UserService.Interface;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMessagePublisher _messagePublisher;

        public UserController(UserManager<User> userManager, IJwtTokenService jwtToken, IMessagePublisher messagePublisher)
        {
            _userManager = userManager;
            _jwtTokenService = jwtToken;
            _messagePublisher = messagePublisher;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegisterModel model)
        {
            var user = new User
            { 
                UserName = model.Username,
                Email = model.Email, 
                FullName = model.FullName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var token = _jwtTokenService.GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized("Invalid login attempt");
        }

        [Authorize]
        [HttpGet("secure")]
        public async Task<IActionResult> UpdateUser(UserUpdateModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found");

            // Validate current password
            if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return BadRequest("Current password is incorrect");
            }

            // Update email and full name
            user.Email = model.Email;
            user.FullName = model.FullName;

            // Update password if a new one is provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            // Update user address
            if (model.Address != null)
            {
                var address = user.UserAddresses.FirstOrDefault(a => a.AddressType == model.Address.AddressType);
                if (address != null)
                {
                    address.AddressLine1 = model.Address.AddressLine1;
                    address.AddressLine2 = model.Address.AddressLine2;
                    address.City = model.Address.City;
                    address.PostalCode = model.Address.PostalCode;
                    address.Country = model.Address.Country;
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return Ok("User updated successfully");
            }

            return BadRequest(updateResult.Errors);
        }

        [Authorize]
        [HttpDelete("user-delete")]
        public async Task<IActionResult> SoftDeleteUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null) return NotFound("User not found");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("User account deactivated");
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}/delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null) return NotFound("User not Found");

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                //Publish user  deletion event to rabbitmq
                var userDeletedMessage = new UserDeleteMessage { UserId = userId, DeletedAt = DateTime.UtcNow };
                await _messagePublisher.PublishAsync(userDeletedMessage, "UserExchange","UserDeleted");

                return Ok("User deleted successfully");
            }
            return BadRequest(result.Errors);
        }
    }
}

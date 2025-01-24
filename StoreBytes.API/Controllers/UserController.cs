using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreBytes.API.Utilities;
using StoreBytes.DataAccess.Data;

namespace StoreBytes.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IDatabaseData _db;

        public UserController(IDatabaseData db)
        {
            _db = db;
        }

        [HttpGet("details")]
        public IActionResult GetUserDetails()
        {
            try
            {
                var userValidationResponse = UserValidationHelper.ValidateUser(User, out int userId);
                if (userValidationResponse != null)
                {
                    return userValidationResponse;
                }

                var user = _db.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

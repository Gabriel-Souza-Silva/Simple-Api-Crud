using System.ComponentModel.DataAnnotations;

namespace SimpleCrud.Models.Request
{
    public class LoginUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

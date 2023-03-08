using System.ComponentModel.DataAnnotations;

namespace SimpleCrud.Models.Request
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

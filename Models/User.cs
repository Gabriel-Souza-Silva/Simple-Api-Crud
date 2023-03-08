using System.Text.Json.Serialization;

namespace SimpleCrud.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public char  Gender { get; set; }
        public string DocumentNumber { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        #region Auth
        [JsonIgnore]
        public string? RefreshToken { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime? TokenCreated { get; set; }
        [JsonIgnore]
        public DateTime? TokenExpires { get; set; }
        [JsonIgnore]
        public string? VerificationToken{ get; set; }
        [JsonIgnore]
        public DateTime? VerifiedAt{ get; set; }
        [JsonIgnore]
        public string? PasswordResetToken { get; set; }
        [JsonIgnore]
        public DateTime? ResetTokenExpires { get; set; }
        [JsonIgnore]
        public byte[]? PasswordHash { get; set; }
        [JsonIgnore]
        public byte[]? PasswordSalt { get; set; }
        #endregion
    }
}

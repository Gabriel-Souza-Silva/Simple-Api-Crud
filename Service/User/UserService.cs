using SimpleCrud.Data;

namespace SimpleCrud.Service.User
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public string Verify(string verificationToken)
        {
            string error = string.Empty;

            var user = _context.Users.FirstOrDefault(u => u.VerificationToken == verificationToken);

            if (user == null)
            {
                error = "Invalid Token or expired";
                return error;
            }

            user.VerifiedAt = DateTime.Now;
            _context.SaveChanges();

            return error;
        }
    }
}

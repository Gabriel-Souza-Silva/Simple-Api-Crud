namespace SimpleCrud.Service.User
{
    public interface IUserService
    {
        string Verify(string verificationToken);
    }
}

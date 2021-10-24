using CORE.Users.Models;

namespace CORE.Users.Interfaces;

public interface IPasswordService : IDisposable
{
    LoginMinModel CheckUser(LoginMinModel login);
}
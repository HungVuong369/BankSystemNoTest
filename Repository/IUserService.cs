using BankSystem.Dtos.Request;
using BankSystem.Models;

namespace BankSystem.Repository
{
    public interface IUserService
    {
        User GetUserByRefreshToken(string refreshToken);
        User GetUserById(long id);
        User GetUserByEmail(string email);
        bool AddUser(UserRequest userRequest);
    }
}

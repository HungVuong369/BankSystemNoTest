using BankSystem.Dtos.Request;
using BankSystem.Dtos.Response;

namespace BankSystem.Repository
{
    public interface IAuthService
    {
        ResponseDto AuthenticateUser(AuthenticateUserRequest authenticateUserRequest);
        ResponseDto RefreshToken(string oldRefreshToken);
        ResponseDto Logout(string refreshToken);
        ResponseDto RevokeRefreshToken();
    }
}

using BankSystem.Data.Repositories;
using BankSystem.Dtos.Request;
using BankSystem.Dtos.Response;
using BankSystem.Repository;

namespace BankSystem.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _authRepository;

        public AuthService(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public ResponseDto RefreshToken(string oldRefreshToken)
        {
            return _authRepository.RefreshToken(oldRefreshToken);
        }

        public ResponseDto AuthenticateUser(AuthenticateUserRequest authenticateUserRequest)
        {
            return _authRepository.AuthenticateUser(authenticateUserRequest);
        }

        public ResponseDto Register(UserRequest userRequest)
        {
            return _authRepository.Register(userRequest);
        }

        public ResponseDto Logout(string refreshToken)
        {
            return _authRepository.Logout(refreshToken);
        }

        public ResponseDto RevokeRefreshToken()
        {
            return _authRepository.RevokeRefreshToken();
        }
    }
}

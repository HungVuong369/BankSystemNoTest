using BankSystem.Data.Repositories;
using BankSystem.Dtos.Request;
using BankSystem.Models;
using BankSystem.Repository;

namespace BankSystem.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public bool AddUser(UserRequest userRequest)
        {
            return _userRepository.AddUser(userRequest);
        }

        public User GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public User GetUserById(long id)
        {
            return _userRepository.GetUserById(id);
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            return _userRepository.GetUserByRefreshToken(refreshToken);
        }
    }
}

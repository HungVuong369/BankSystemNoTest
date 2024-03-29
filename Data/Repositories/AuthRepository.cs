using BankSystem.Dtos.Request;
using BankSystem.Dtos.Response;
using BankSystem.Models;
using BankSystem.Service;
using BankSystem.Utilities;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankSystem.Data.Repositories
{
    public class AuthRepository
    {
        private readonly string _jwtSecret;
        private readonly int _jwtExpirationMinutes;
        private readonly string _issuer;
        private readonly string _Audience;
        private readonly UserService _userService;
        private readonly PasswordHashingService _passwordHashingService;
        private readonly IDataAccess _dataAccess;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AuthRepository(UserService userService, PasswordHashingService passwordHashingService, IDataAccess dataAccess, IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this._userService = userService;
            this._dataAccess = dataAccess;
            this._passwordHashingService = passwordHashingService;
#pragma warning disable CS8601 // Possible null reference assignment.
            _jwtSecret = configuration["Jwt:Key"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.
            _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"]);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.
            _issuer = configuration["Jwt:Issuer"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
            _Audience = configuration["Jwt:Audience"];
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            // Config access token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _Audience,
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(_jwtExpirationMinutes),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string getUniqueToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var query = "SELECT * FROM RefreshToken WHERE refreshToken = @refreshToken";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@refreshToken", token),
            };

            var dataTable = _dataAccess.ExecuteQuery(query, parameters);

            var tokenIsUnique = false;

            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                tokenIsUnique = true;
            }

            if (!tokenIsUnique)
                return getUniqueToken();

            return token;
        }

        public RefreshTokenResponse GenerateRefreshToken()
        {
            // Config refresh token

            var refreshToken = new RefreshTokenResponse
            {
                Token = getUniqueToken(),
                Expires = DateTime.Now.AddMinutes(2),
                Created = DateTime.Now,
            };

            return refreshToken;
        }

        public bool AddRefreshToken(long userId, string refreshToken)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@refreshToken", refreshToken);
                parameters.Add("@userId", userId);
                // Config refresh token
                parameters.Add("@expireDate", DateTime.Now.AddMinutes(2));

                var isCheck = _dataAccess.ExecuteStoredProcedure<int>("AddRefreshToken", parameters);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRefreshToken(long userId, string refreshToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@refreshToken", refreshToken);
            // Config refresh token
            parameters.Add("@expireDate", DateTime.Now.AddMinutes(2));

            var isCheck = _dataAccess.ExecuteStoredProcedure<int>("UpdateRefreshToken", parameters);

            return isCheck == 1 ? true : false;
        }

        public bool IsCheckExpireRefreshToken_ByUserId(long userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId);

            var isCheck = _dataAccess.ExecuteStoredProcedure<int>("IsCheckExpireRefreshToken_ByUserId", parameters);

            // 0 = chưa hết hạn, 1 = đã hết hạn hoặc token không tồn tại
            return isCheck == 0 ? false : true;
        }

        public DateTime getExpireDateTimeRefreshToken_ByUserId(long userId)
        {
            string query = "SELECT expireDate FROM RefreshToken WHERE RefreshToken.userId = @userId";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@userId", userId),
            };

            var dataTable = _dataAccess.ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count == 0)
                return DateTime.Now;

            var row = dataTable.Rows[0];
            var expireDate = row["expireDate"];

            return (DateTime)expireDate;
        }

        public string GetRefreshToken_ByUserId(long userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);

            var refreshToken = _dataAccess.ExecuteStoredProcedure<string>("GetRefreshToken_ByUserId", parameters);

            return refreshToken;
        }

        public ResponseDto AuthenticateUser(AuthenticateUserRequest authenticateUserRequest)
        {
            var user = _userService.GetUserByEmail(authenticateUserRequest.email);

            if (user == null)
            {
                return HelperFunctions.Instance.GetErrorResponseByError(17);
            }
            bool passwordMatch = _passwordHashingService.VerifyPassword(authenticateUserRequest.password, user.Password);

            if (!passwordMatch)
            {
                return HelperFunctions.Instance.GetErrorResponseByError(17);
            }

            var jwtToken = GenerateJwtToken(user);
            string refreshToken = null;

            if (IsCheckExpireRefreshToken_ByUserId(user.Id))
            {
                refreshToken = GenerateRefreshToken().Token;
                if (!UpdateRefreshToken(user.Id, refreshToken))
                {
                    AddRefreshToken(user.Id, refreshToken);
                }
            }
            else
            {
                refreshToken = GetRefreshToken_ByUserId(user.Id);
            }

            return new ResponseDto(new UserTokenResponse(jwtToken, refreshToken, user.Email, user.Role, getExpireDateTimeRefreshToken_ByUserId(user.Id)));
        }

        public ResponseDto RefreshToken(string refreshToken)
        {
            var user = _userService.GetUserByRefreshToken(refreshToken);

            if (user == null)
            {
                return new ResponseDto(401, "Invalid Token");
            }
            var jwtToken = GenerateJwtToken(user);

            // Đã hết hạn
            if (IsCheckExpireRefreshToken_ByUserId(user.Id))
            {
                refreshToken = GenerateRefreshToken().Token;
                if (!UpdateRefreshToken(user.Id, refreshToken))
                {
                    AddRefreshToken(user.Id, refreshToken);
                }
            }

            return new ResponseDto(new UserTokenResponse(jwtToken, refreshToken, user.Email, user.Role, getExpireDateTimeRefreshToken_ByUserId(user.Id)));
        }

        public ResponseDto Register(UserRequest userRequest)
        {
            string password = userRequest.Password;
            userRequest.Password = _passwordHashingService.HashPassword(password);

            bool isCheck = _userService.AddUser(userRequest);

            if (!isCheck)
            {
                return new ResponseDto(1001, "Username already exists");
            }

            var user = _userService.GetUserByEmail(userRequest.Email);

            var jwtToken = GenerateJwtToken(user);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string refreshToken = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            refreshToken = GenerateRefreshToken().Token;
            AddRefreshToken(user.Id, refreshToken);

            return new ResponseDto(new UserTokenResponse(jwtToken, refreshToken, userRequest.Email, user.Role, getExpireDateTimeRefreshToken_ByUserId(user.Id)));
        }

        public ResponseDto Logout(string refreshToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_refreshToken", refreshToken, DbType.String, ParameterDirection.Input);

            var isCheck = _dataAccess.ExecuteStoredProcedure<int>("DeleteRefreshToken", parameters);

            if (isCheck == 1)
                return new ResponseDto();
            return new ResponseDto(1001, "Logout failed");
        }

        public ResponseDto RevokeRefreshToken()
        {
            var parameters = new DynamicParameters();

            var isCheck = _dataAccess.ExecuteStoredProcedure<int>("RevokeRefreshToken", parameters);

            if (isCheck == 0)
            {
                return new ResponseDto(-1, "Error!");
            }

            return new ResponseDto();
        }
    }
}

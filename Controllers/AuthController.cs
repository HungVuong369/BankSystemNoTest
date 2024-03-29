using BankSystem.Dtos.Request;
using BankSystem.Dtos.Response;
using BankSystem.Service;
using BankSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthService> _logger;

        public AuthController(AuthService authService, ILogger<AuthService> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticateUser")]
        public IActionResult AuthenticateUser([FromBody] AuthenticateUserRequest authenticateUserRequest)
        {
            try
            {
                var result = _authService.AuthenticateUser(authenticateUserRequest);

                if (result.ErrorCode == HelperFunctions.ErrorCodeWrongUsernameOrPassword)
                {
                    return LogErrorAndReturnBadRequest(result);
                }

                ManageRefreshToken(result);

                _logger.LogInformation("AuthenticateUser method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return LogExceptionAndReturnServerError(ex);
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(string refreshToken)
        {
            var result = _authService.RefreshToken(refreshToken);

            if (result.ErrorCode != HelperFunctions.Success)
            {
                return LogErrorAndReturnBadRequest(result);
            }

            _logger.LogInformation("RefreshToken method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

            return Ok(result);
        }

        private IActionResult LogErrorAndReturnBadRequest(ResponseDto result)
        {
            _logger.LogInformation("Error occurred: " + HelperFunctions.Instance.ConvertObjectToJson(result));
            return BadRequest(HelperFunctions.Instance.GetErrorResponseByError(result.ErrorCode));
        }
        
        private IActionResult LogExceptionAndReturnServerError(Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
            return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
        }

        private void ManageRefreshToken(ResponseDto result)
        {
            if (Request.Cookies["refreshToken"] == null)
            {
                setTokenCookie((result.Data as UserTokenResponse).RefreshToken);
            }
            else
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var responseRefreshToken = (result.Data as UserTokenResponse).RefreshToken;

                if (refreshToken != responseRefreshToken)
                {
                    setTokenCookie((result.Data as UserTokenResponse).RefreshToken);
                }
                else
                {
                    setTokenCookie((result.Data as UserTokenResponse).RefreshToken, (result.Data as UserTokenResponse).ExpireDateTimeRefreshToken);
                }
            }
        }

        private void setTokenCookie(string refreshToken, DateTime? expireDateTime = null)
        {
            CookieOptions cookieOptions = null;

            // Config refresh token
            if (expireDateTime == null)
            {
                cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(2),
                };
            }
            else
            {
                TimeSpan timeDifference = expireDateTime.Value - DateTime.Now;
                int secondsDifference = (int)timeDifference.TotalSeconds;

                cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddSeconds(secondsDifference),
                };
            }

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRequest userRequest)
        {
            var result = _authService.Register(userRequest);
            _logger.LogInformation("Register method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
            return StatusCode(result.ErrorCode, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var result = _authService.Logout(Request.Cookies["refreshToken"].ToString());

            if (result.ErrorCode == 0)
            {
                Response.Cookies.Delete("refreshToken");
                Response.Cookies.Delete("accessToken");
            }
            _logger.LogInformation("Logout method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

            return StatusCode(result.ErrorCode, result);
        }
    }
}

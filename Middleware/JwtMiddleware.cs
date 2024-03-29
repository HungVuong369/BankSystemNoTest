using BankSystem.Data.Repositories;
using BankSystem.Dtos.Response;
using System.IdentityModel.Tokens.Jwt;

namespace BankSystem.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private AuthRepository _AuthRepository = null;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_AuthRepository == null)
            {
                _AuthRepository = context.RequestServices.GetRequiredService<AuthRepository>();
            }

            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var accessTokenTemp = context.Request.Cookies["accessToken"];

            if (accessTokenTemp != null)
            {
                AttachUserToContext(context, accessTokenTemp.ToString());
            }
            else if (accessToken != null)
            {
                var isCheck = AttachUserToContext(context, accessToken);

                // Refresh token hết hạn
                if (isCheck)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Refresh Token has expired");
                }
            }

            await _next(context);
        }

        private void setTokenCookie(HttpContext context, string accessToken)
        {
            // Config access token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(1),
            };
            context.Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        }

        private bool AttachUserToContext(HttpContext context, string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);
            var expirationTime = token.ValidTo.ToLocalTime();

            var refreshToken = context.Request.Cookies["refreshToken"];

            // Refresh token hết hạn
            if (refreshToken == null)
            {
                _AuthRepository.RevokeRefreshToken();
                context.Response.Cookies.Delete("refreshToken");
                context.Response.Cookies.Delete("accessToken");
                return true;
            }

            // Access token hết hạn
            if (DateTime.Now > expirationTime)
            {
                var response = _AuthRepository.RefreshToken(refreshToken);
                var userTokenResponse = (response.Data as UserTokenResponse);

                if (response.ErrorCode == 0)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var newAccessToken = userTokenResponse.Token;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    context.Request.Headers["Authorization"] = "Bearer " + userTokenResponse.Token;
                    setTokenCookie(context, userTokenResponse.Token);
                }
            }
            else
            {
                var accessTokenCookie = context.Request.Cookies["accessToken"];

                if (accessTokenCookie != null)
                    context.Request.Headers["Authorization"] = "Bearer " + accessTokenCookie;
            }
            return false;
        }
    }
}

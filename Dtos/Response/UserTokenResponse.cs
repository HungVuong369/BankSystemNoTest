namespace BankSystem.Dtos.Response
{
    public class UserTokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpireDateTimeRefreshToken { get; set; }

        public UserTokenResponse(string token, string refreshToken, string email, string role, DateTime expireDateTimeRefreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
            Email = email;
            Role = role;
            ExpireDateTimeRefreshToken = expireDateTimeRefreshToken;
        }
    }
}

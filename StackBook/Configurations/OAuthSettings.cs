namespace StackBook.Configurations
{
    public class GoogleOAuthConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        public string AuthUrl { get; set; }              // https://accounts.google.com/o/oauth2/v2/auth
        public string TokenUrl { get; set; }             // https://oauth2.googleapis.com/token
        public string UserInfoUrl { get; set; }          // https://www.googleapis.com/oauth2/v2/userinfo

        public List<string> Scopes { get; set; }         // Ví dụ: ["openid", "profile", "email"]
        public GoogleOAuthConfig()
        {
        }
    }
}

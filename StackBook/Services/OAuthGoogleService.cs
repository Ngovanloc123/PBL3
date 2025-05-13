using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using StackBook.Configurations;
using System.Text.Json;
using System.Web;

namespace StackBook.Services
{
    public class OAuthGoogleService
    {
        private readonly GoogleOAuthConfig _config;

        public OAuthGoogleService(IOptions<GoogleOAuthConfig> config)
        {
            _config = config.Value;
        }

        public async Task<string> GetRedirectConsentScreenURL()
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["client_id"] = _config.ClientId;
            queryParams["redirect_uri"] = _config.RedirectUri;
            queryParams["response_type"] = "code";
            queryParams["access_type"] = "offline";
            queryParams["scope"] = string.Join(" ", _config.Scopes);
            queryParams["state"] = "some_state";
            string url = $"{_config.AuthUrl}?{queryParams}";
            Console.WriteLine($"Google Consent screen url: {url}");
            return await Task.FromResult(url);
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            using var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _config.ClientId },
                { "client_secret", _config.ClientSecret },
                { "redirect_uri", _config.RedirectUri },
                { "grant_type", "authorization_code" }
            });

            var response = await client.PostAsync(_config.TokenUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<JsonElement>(responseContent);

            return token.GetProperty("access_token").GetString();
        }

        public async Task<(string Email, string Name, string GoogleId)> GetGoogleUserProfileAsync(string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            Console.WriteLine($"Access token: {accessToken}");
            Console.WriteLine($"User info URL: {_config.UserInfoUrl}");
            Console.WriteLine($"Authorization header: {client.DefaultRequestHeaders.Authorization}");
            Console.WriteLine($"Accept headers: {string.Join(", ", client.DefaultRequestHeaders.Accept)}");
            Console.WriteLine($"User info URL: {_config.UserInfoUrl}");
            var result = await client.GetStringAsync(_config.UserInfoUrl);
            Console.WriteLine($"User info result: {result}");
            var profile = JsonSerializer.Deserialize<JsonElement>(result);

            return (
                profile.GetProperty("email").GetString()!,
                profile.GetProperty("name").GetString()!,
                profile.GetProperty("id").GetString()! // Google ID
            );
        }
    }
}

using System.Text;
using System.Text.Json;
using MAUI_LKS2.Models;

namespace MAUI_LKS2.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AuthService()
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                _baseUrl = "http://10.0.2.2:7134/api/";
            }
            else
            {
                _baseUrl = "https://localhost:7134/api/";
            }

            var handler = new HttpClientHandler();
            
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                handler.ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true;
            }

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<User>(responseJson);
                    return user;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Login error: {response.StatusCode} - {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Register error: {response.StatusCode} - {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register exception: {ex.Message}");
                return false;
            }
        }
    }
}
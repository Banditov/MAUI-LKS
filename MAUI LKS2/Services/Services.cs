using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MAUI_LKS2.Models;

namespace MAUI_LKS2.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(string baseUrl)
        {
            _baseUrl = baseUrl;

            // ✅ Configure HttpClientHandler for Android
            var handler = new HttpClientHandler();

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // ✅ Accept all SSL certificates (for development only)
                handler.ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true;

                // ✅ Use the default SSL/TLS settings
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            }

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // GET: Get all sales
        public async Task<List<Sale>?> GetSalesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sales");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Sale>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sales: {ex.Message}");
                return null;
            }
        }

        // GET: Get sale by id
        public async Task<Sale?> GetSaleAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"sales/{id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Sale>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sale: {ex.Message}");
                return null;
            }
        }

        // POST: Create new sale
        public async Task<Sale?> CreateSaleAsync(CreateSaleDto sale)
        {
            try
            {
                var json = JsonSerializer.Serialize(sale);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ Log what you're sending
                System.Diagnostics.Debug.WriteLine($"=== API CALL ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_httpClient.BaseAddress}sales");
                System.Diagnostics.Debug.WriteLine($"Data: {json}");

                var response = await _httpClient.PostAsync("sales", content);

                // ✅ Log the response
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<Sale>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"STACK: {ex.StackTrace}");
                return null;
            }
        }

        // PUT: Update sale
        public async Task<bool> UpdateSaleAsync(int id, UpdateSaleDto sale)
        {
            try
            {
                var json = JsonSerializer.Serialize(sale);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"sales/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating sale: {ex.Message}");
                return false;
            }
        }

        // DELETE: Delete sale
        public async Task<bool> DeleteSaleAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"sales/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting sale: {ex.Message}");
                return false;
            }
        }

        // GET: Search sales
        public async Task<List<Sale>?> SearchSalesAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"sales/search?q={Uri.EscapeDataString(query)}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Sale>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching sales: {ex.Message}");
                return null;
            }
        }

        // GET: Export CSV
        public async Task<byte[]?> ExportSalesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sales/export");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting sales: {ex.Message}");
                return null;
            }
        }
    }
}
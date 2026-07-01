using MAUI_LKS2.Models;
using MAUI_LKS2.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAUI_LKS2.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<Sale> _products = new();
        private bool _isLoading;

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Sale> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public SalesViewModel()
        {
            string baseUrl;

            IsAdmin = App.CurrentUser?.IsAdmin ?? false;

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                baseUrl = "https://10.0.2.2:7134/api/";
            }
            else
            {
                baseUrl = "https://localhost:7134/api/";
            }

            _apiService = new ApiService(baseUrl);
            Products = new ObservableCollection<Sale>();
            LoadProducts();
        }

        public async Task LoadProducts()
        {
            IsLoading = true;
            try
            {
                var sales = await _apiService.GetSalesAsync();
                if (sales != null)
                {
                    Products = new ObservableCollection<Sale>(sales);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading products: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task<bool> AddProduct(string productName, decimal price, int sales)
        {
            try
            {
                var dto = new CreateSaleDto
                {
                    ProductName = productName,
                    Price = price,
                    Sales = sales
                };

                var result = await _apiService.CreateSaleAsync(dto);
                if (result != null)
                {
                    await LoadProducts();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var result = await _apiService.DeleteSaleAsync(id);
                if (result)
                {
                    await LoadProducts();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateProduct(Sale sale)
        {
            try
            {
                var dto = new UpdateSaleDto
                {
                    ProductName = sale.ProductName,
                    Price = sale.Price,
                    Sales = sale.Sales
                };

                var result = await _apiService.UpdateSaleAsync(sale.Id, dto);
                if (result)
                {
                    await LoadProducts();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Sale>?> SearchProducts(string query)
        {
            try
            {
                return await _apiService.SearchSalesAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                return null;
            }
        }
        public async Task<byte[]?> ExportProducts()
        {
            try
            {
                return await _apiService.ExportSalesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export error: {ex.Message}");
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
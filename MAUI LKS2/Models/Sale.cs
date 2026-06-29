using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MAUI_LKS2.Models
{
    public class Sale : INotifyPropertyChanged
    {
        private int _id;
        private string _productName = string.Empty;
        private decimal _price;
        private int _sales;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt;
        private bool _isEditing;
        private string _editProductName = string.Empty;
        private string _editPrice = string.Empty;
        private string _editSales = string.Empty;

        [JsonPropertyName("id")]
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("productName")]
        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("price")]
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PriceDisplay));
            }
        }

        [JsonPropertyName("sales")]
        public int Sales
        {
            get => _sales;
            set { _sales = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(); }
        }

        public string PriceDisplay => $"${Price:F2}";

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                if (value)
                {
                    EditProductName = ProductName;
                    EditPrice = Price.ToString();
                    EditSales = Sales.ToString();
                }
            }
        }

        public string EditProductName
        {
            get => _editProductName;
            set { _editProductName = value; OnPropertyChanged(); }
        }

        public string EditPrice
        {
            get => _editPrice;
            set { _editPrice = value; OnPropertyChanged(); }
        }

        public string EditSales
        {
            get => _editSales;
            set { _editSales = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
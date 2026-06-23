using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MAUI_LKS2.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Sales { get; set; }

        public string PriceDisplay => $"{Price:F2}";

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public string EditProductName { get; set; }
        public string EditPrice { get; set; }
        public string EditSales { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
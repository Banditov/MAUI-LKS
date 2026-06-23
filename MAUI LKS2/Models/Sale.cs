using System;
using System.Collections.Generic;
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
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAUILKSAPI.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Sales { get; set; }
    }
}

namespace MAUILKSAPI.Models
{
    public class UpdateSaleDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Sales { get; set; }
    }
}

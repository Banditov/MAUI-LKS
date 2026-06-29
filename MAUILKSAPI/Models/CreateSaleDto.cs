namespace MAUILKSAPI.Models
{
    public class CreateSaleDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Sales { get; set; }
    }
}

using MAUILKSAPI.Models;
using MAUILKSAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUILKSAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        return await _context.Sales.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
            return NotFound();
        return sale;
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> CreateSale([FromBody] CreateSaleDto dto)
    {
        var sale = new Sale
        {
            ProductName = dto.ProductName,
            Price = dto.Price,
            Sales = dto.Sales,
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(int id, [FromBody] UpdateSaleDto dto)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
            return NotFound();

        sale.ProductName = dto.ProductName;
        sale.Price = dto.Price;
        sale.Sales = dto.Sales;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
            return NotFound();

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Sale>>> SearchSales([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return await GetSales();

        var results = await _context.Sales
            .Where(s => s.ProductName.Contains(q))
            .ToListAsync();

        return results;
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportSales()
    {
        var sales = await _context.Sales.ToListAsync();
        var csv = "Id,ProductName,Price,Sales,CreatedAt\n";
        csv += string.Join("\n", sales.Select(s =>
            $"{s.Id},{s.ProductName},{s.Price},{s.Sales}"));

        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv",
            $"sales_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}
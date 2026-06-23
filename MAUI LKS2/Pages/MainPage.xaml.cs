using MAUI_LKS2.Models;
using MAUI_LKS2.ViewModels;
using Microsoft.Data.SqlClient;

namespace MAUI_LKS2.Pages;

public partial class MainPage : ContentPage
{
    private List<Sale> _allProducts = new List<Sale>();
    private SalesViewModel _viewModel;
    private string _selectedFilter = "Price";

    public MainPage()
	{
		InitializeComponent();
        _viewModel = new SalesViewModel();
        BindingContext = _viewModel;
        LoadProducts();
    }

    private async void LoadProducts()
    {
        await _viewModel.LoadProducts();
        _allProducts = _viewModel.Products.ToList();
        ProductsCollection.ItemsSource = _allProducts;
    }

    private async void AddBtnClicked (object? sender, EventArgs e)
	{
		string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

		string ProductName = ProductNameEntry.Text;
		int Price = Convert.ToInt32(PriceEntry.Text);
		int Sales = Convert.ToInt32(SalesEntry.Text);

		try 
		{
			using SqlConnection connection = new(conn);
			await connection.OpenAsync();

			string CheckQuery = "SELECT Count(*) FROM sales WHERE product_name = @ProductName";
			using SqlCommand check = new(CheckQuery, connection);
			check.Parameters.AddWithValue("@ProductName", ProductName);

			int CheckCount = (int)await check.ExecuteScalarAsync();

			if (CheckCount > 0)
			{
				await DisplayAlertAsync("Error", "Product already exist", "OK");
				return;
			}

			string InsertQuery = "INSERT INTO sales (product_name, price, sales) VALUES (@ProductName, @Price, @Sales)";
			using SqlCommand insert = new(InsertQuery, connection);
			insert.Parameters.AddWithValue("@ProductName", ProductName);
			insert.Parameters.AddWithValue("@Price", Price);
			insert.Parameters.AddWithValue("@Sales", Sales);

            int rowsAffected = await insert.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await DisplayAlertAsync("Success", "Product added!", "OK");
                await Shell.Current.GoToAsync("Pages/MainPage");
                return;
            }
            await DisplayAlertAsync("Error", "Failed to add product", "OK");
        }
		catch(Exception ex)
		{
			await DisplayAlertAsync("Error", ex.Message, "OK");
		}
    }

    private void DatabaseBtnClicked(object? sender, EventArgs e)
    {

    }

	private async void DeleteBtnClicked(object? sender, EventArgs e)
	{
        var button = (Button)sender;
        int productId = (int)button.CommandParameter;

        bool confirm = await DisplayAlertAsync("Confirm Delete",
            "Are you sure you want to delete this product?",
            "Yes", "No");

        if (!confirm) return;

        try
        {
            bool success = await ((SalesViewModel)BindingContext).DeleteProduct(productId);

            if (success)
            {
                await DisplayAlertAsync("Success", "Product deleted successfully!", "OK");
            }
            await DisplayAlertAsync("Error", "Failed to delete product", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Delete failed: {ex.Message}", "OK");
        }
    }

    private async void EditBtnClicked(object? sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = button.BindingContext as Sale;
        product.IsEditing = true;
        RefreshCollection();
    }

    private async void SaveBtnClicked(object? sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = button.BindingContext as Sale;
        int productId = (int)button.CommandParameter;

        string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

        var parentGrid = button.Parent as Grid;
        var nameEntry = parentGrid.FindByName<Entry>("ProductNameEntryEdit");
        var priceEntry = parentGrid.FindByName<Entry>("PriceEntryEdit");
        var salesEntry = parentGrid.FindByName<Entry>("SalesEntryEdit");

        string ProductName = nameEntry.Text;
        decimal Price = Convert.ToDecimal(priceEntry.Text);
        int Sales = Convert.ToInt32(salesEntry.Text);

        try
        {
            using SqlConnection connection = new(conn);
            await connection.OpenAsync();

            string UpdateQuery = "UPDATE sales SET product_name = @ProductName, price = @Price, sales = @Sales WHERE id = @Id";
            using SqlCommand update = new(UpdateQuery, connection);
            update.Parameters.AddWithValue("@ProductName", ProductName);
            update.Parameters.AddWithValue("@Price", Price);
            update.Parameters.AddWithValue("@Sales", Sales);
            update.Parameters.AddWithValue("@Id", productId);

            int rowsAffected = await update.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await DisplayAlertAsync("Success", "Product edited!", "OK");
                await Shell.Current.GoToAsync("Pages/MainPage");
                return;
            }
            await DisplayAlertAsync("Error", "Failed to edit product", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }

        product.IsEditing = false;
        RefreshCollection();
    }

    private void OnFilterPickerSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        _selectedFilter = picker.SelectedItem?.ToString() ?? "Price";
    }

    private async void FilterApply(object? sender, EventArgs e)
    {
        string search = SearchBar.Text ?? "";

        int min = 0;
        int max = int.MaxValue;

        if (!string.IsNullOrWhiteSpace(MinEntry.Text))
        {
            int.TryParse(MinEntry.Text, out min);
        }

        if (!string.IsNullOrWhiteSpace(MaxEntry.Text))
        {
            int.TryParse(MaxEntry.Text, out max);
        }

        var filtered = (_allProducts ?? Enumerable.Empty<Sale>()).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(p =>
                p.ProductName.ToLower().Contains(search.ToLower())
            );
        }

        switch (_selectedFilter)
        {
            case "No.":
                filtered = filtered.Where(p => p.Id >= min && p.Id <= max);
                break;
            case "Price":
                filtered = filtered.Where(p => p.Price >= min && p.Price <= max);
                break;
            case "Sales":
                filtered = filtered.Where(p => p.Sales >= min && p.Sales <= max);
                break;
            default:
                filtered = filtered.Where(p => p.Price >= min && p.Price <= max);
                break;
        }

        ProductsCollection.ItemsSource = filtered.ToList();
        int count = filtered.Count();
    }

    private async void FilterReset(object? sender, EventArgs e)
    {
        ProductsCollection.ItemsSource = _allProducts;
    }

    private void RefreshCollection()
    {
        var currentSource = ProductsCollection.ItemsSource;
        ProductsCollection.ItemsSource = null;
        ProductsCollection.ItemsSource = currentSource;
    }
}
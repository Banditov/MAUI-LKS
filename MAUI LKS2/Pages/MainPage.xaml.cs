using MAUI_LKS2.Models;
using MAUI_LKS2.ViewModels;

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

    private async Task LoadProducts()
    {
        await _viewModel.LoadProducts();
        _allProducts = _viewModel.Products.ToList();
        ProductsCollection.ItemsSource = _allProducts;
    }

    private async void AddBtnClicked(object? sender, EventArgs e)
    {
        try
        {
            string productName = ProductNameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(productName))
            {
                await DisplayAlertAsync("Error", "Please enter a product name", "OK");
                return;
            }

            if (!decimal.TryParse(PriceEntry.Text, out decimal price))
            {
                await DisplayAlertAsync("Error", "Please enter a valid price", "OK");
                return;
            }

            if (!int.TryParse(SalesEntry.Text, out int sales))
            {
                await DisplayAlertAsync("Error", "Please enter a valid sales number", "OK");
                return;
            }

            bool success = await _viewModel.AddProduct(productName, price, sales);

            if (success)
            {
                await DisplayAlertAsync("Success", "Product added!", "OK");

                ProductNameEntry.Text = "";
                PriceEntry.Text = "";
                SalesEntry.Text = "";

                await LoadProducts();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to add product. Product may already exist.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
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
            bool success = await _viewModel.DeleteProduct(productId);

            if (success)
            {
                await DisplayAlertAsync("Success", "Product deleted successfully!", "OK");
                await LoadProducts();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to delete product", "OK");
            }
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
        if (product == null) return;

        product.EditProductName = product.ProductName;
        product.EditPrice = product.Price.ToString();
        product.EditSales = product.Sales.ToString();

        product.IsEditing = true;
        RefreshCollection();
    }

    private async void SaveBtnClicked(object? sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = button.BindingContext as Sale;
        if (product == null) return;

        try
        {
            var parentGrid = button.Parent as Grid;
            if (parentGrid == null) return;

            var nameEntry = parentGrid.FindByName<Entry>("ProductNameEntryEdit");
            var priceEntry = parentGrid.FindByName<Entry>("PriceEntryEdit");
            var salesEntry = parentGrid.FindByName<Entry>("SalesEntryEdit");

            if (nameEntry == null || priceEntry == null || salesEntry == null)
            {
                await DisplayAlertAsync("Error", "Could not find entry fields", "OK");
                return;
            }

            string productName = nameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(productName))
            {
                await DisplayAlertAsync("Error", "Product name cannot be empty", "OK");
                return;
            }

            if (!decimal.TryParse(priceEntry.Text, out decimal price))
            {
                await DisplayAlertAsync("Error", "Please enter a valid price", "OK");
                return;
            }

            if (!int.TryParse(salesEntry.Text, out int sales))
            {
                await DisplayAlertAsync("Error", "Please enter a valid sales number", "OK");
                return;
            }

            product.ProductName = productName;
            product.Price = price;
            product.Sales = sales;

            bool success = await _viewModel.UpdateProduct(product);

            if (success)
            {
                await DisplayAlertAsync("Success", "Product updated!", "OK");
                product.IsEditing = false;
                await LoadProducts();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to update product", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void OnFilterPickerSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        _selectedFilter = picker.SelectedItem?.ToString() ?? "Price";
    }
    private async void FilterApply(object? sender, EventArgs e)
    {
        string search = SearchBar.Text?.Trim() ?? "";

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
        SearchBar.Text = "";
        MinEntry.Text = "";
        MaxEntry.Text = "";
        ProductsCollection.ItemsSource = _allProducts;
    }

    private void RefreshCollection()
    {
        var currentSource = ProductsCollection.ItemsSource;
        ProductsCollection.ItemsSource = null;
        ProductsCollection.ItemsSource = currentSource;
    }

    private async void ExportBtnClicked(object? sender, EventArgs e)
    {
        try
        {
            var csvData = await _viewModel.ExportProducts();

            if (csvData == null || csvData.Length == 0)
            {
                await DisplayAlertAsync("Error", "No data to export", "OK");
                return;
            }

            string fileName = $"Sales_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            await File.WriteAllBytesAsync(filePath, csvData);

            await ShareFileAsync(filePath, fileName);

            await DisplayAlertAsync("Success", $"Database exported to {fileName}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Export failed: {ex.Message}", "OK");
        }
    }

    private async Task ShareFileAsync(string filePath, string fileName)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Export Database",
            File = new ShareFile(filePath)
        });
    }

    private async void ImportBtnClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("DEBUG", "IMPORT PLACEHOLDER", "OK");
    }
}
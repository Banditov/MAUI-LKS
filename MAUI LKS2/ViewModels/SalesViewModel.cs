using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_LKS2.Models;

namespace MAUI_LKS2.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Sale> _products;
        public ObservableCollection<Sale> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public SalesViewModel()
        {
            Products = new ObservableCollection<Sale>();
            LoadProducts();
        }

        public async Task LoadProducts()
        {
            string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

            try
            {
                using SqlConnection connection = new(conn);
                await connection.OpenAsync();

                string query = "SELECT id, product_name, price, sales FROM sales ORDER BY id";
                using SqlCommand cmd = new(query, connection);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                Products.Clear();
                while (await reader.ReadAsync())
                {
                    Products.Add(new Sale
                    {
                        Id = reader.GetInt32(0),
                        ProductName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Sales = reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<bool> AddProduct(string productName, decimal price, int sales)
        {
            string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

            try
            {
                using SqlConnection connection = new(conn);
                await connection.OpenAsync();

                string checkQuery = "SELECT COUNT(*) FROM sales WHERE product_name = @ProductName";
                using SqlCommand checkCmd = new(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@ProductName", productName);

                int count = (int)await checkCmd.ExecuteScalarAsync();
                if (count > 0)
                {
                    return false;
                }

                string insertQuery = "INSERT INTO sales (product_name, price, sales) VALUES (@ProductName, @Price, @Sales)";
                using SqlCommand insertCmd = new(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@ProductName", productName);
                insertCmd.Parameters.AddWithValue("@Price", price);
                insertCmd.Parameters.AddWithValue("@Sales", sales);

                int rowsAffected = await insertCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> DeleteProduct(int id)
        {
            string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

            try
            {
                using SqlConnection connection = new(conn);
                await connection.OpenAsync();

                string query = "DELETE FROM sales WHERE id = @Id";
                using SqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    var productToRemove = Products.FirstOrDefault(p => p.Id == id);
                    if (productToRemove != null)
                    {
                        Products.Remove(productToRemove);
                    }
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

        public async Task Filter()
        {

        }
    }
}
using Microsoft.Data.SqlClient;

namespace MAUI_LKS2.Pages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();

		EmailEntry.TextChanged += OnFormChange;
		PasswordEntry.TextChanged += OnFormChange;
	}

	private void OnFormChange (object? sender, EventArgs e)
	{
		bool isActive = !string.IsNullOrEmpty(EmailEntry.Text) &&
						!string.IsNullOrEmpty(PasswordEntry.Text);

		LoginBtn.IsEnabled = isActive;
    }

	private async void OnLoginBtnClicked(object? sender, EventArgs e)
	{
		string email = EmailEntry.Text;
		string password = PasswordEntry.Text;

        string conn = @"Data Source=(localdb)\MSSQLLOCALDB;Initial Catalog=MAUILKS;Integrated Security=True;";

		try
        {
            using SqlConnection connection = new(conn);
            await connection.OpenAsync();

			string findQuery = "SELECT password FROM accounts WHERE email = @email;";

			using SqlCommand find = new(findQuery, connection);
			find.Parameters.AddWithValue("@email", email);

            object? result = await find.ExecuteScalarAsync();

            if (result == null)
            {
                await DisplayAlertAsync("Login Failed", "Invalid email or password", "OK");
                return;
            }

            string storedHash = result.ToString()!;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);

            if (isValid)
            {
                await DisplayAlertAsync("Success", "Login successful!", "OK");
                await Shell.Current.GoToAsync("/Pages/MainPage");
            }
            else
            {
                await DisplayAlertAsync("Login Failed", "Invalid email or password", "OK");
            }
        }
		catch (Exception ex)
		{
			await DisplayAlertAsync("Error", $"Database connection failed: {ex.Message}", "OK");
        }
    }

	private void OnEyeBtnClicked(object? sender, EventArgs e)
	{
        if (PasswordEntry.IsPassword == true)
		{
			PasswordEntry.IsPassword = false;
			PasswordShow.BackgroundColor = Color.FromArgb("#464646");
        }
		else
		{
			PasswordEntry.IsPassword = true;
			PasswordShow.BackgroundColor = Color.FromArgb("#7676ED");
		}
	}

	private async void OnRegisterBtnClicked (object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Pages/Register");
	}
}
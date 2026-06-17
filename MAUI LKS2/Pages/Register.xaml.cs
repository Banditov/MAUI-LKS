using Microsoft.Data.SqlClient;

namespace MAUI_LKS2.Pages;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();

        PasswordEntry.TextChanged += OnFormInput;
        PasswordEntryConf.TextChanged += OnFormInput;
        EmailEntry.TextChanged += OnFormInput;
	}

    private void OnFormInput(object? sender, EventArgs e)
    {
        bool isActive = !string.IsNullOrEmpty(EmailEntry.Text) &&
                        !string.IsNullOrEmpty(PasswordEntry.Text) &&
                        !string.IsNullOrEmpty(PasswordEntryConf.Text);

        RegisterBtn.IsEnabled = isActive;
    }

    private async void OnLoginBtnClicked (object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Pages/Login");
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

    private void OnEyeBtnClickedConf(object? sender, EventArgs e)
    {
        if (PasswordEntryConf.IsPassword == true)
        {
            PasswordEntryConf.IsPassword = false;
            PasswordShowConf.BackgroundColor = Color.FromArgb("#464646");
        }
        else
        {
            PasswordEntryConf.IsPassword = true;
            PasswordShowConf.BackgroundColor = Color.FromArgb("#7676ED");
        }
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private async void OnRegisterBtnClicked(object? sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string passwordConf = PasswordEntryConf.Text;

        string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MAUILKS;Integrated Security=True;";

        if (password == passwordConf)
        {
            try
            {
                using SqlConnection connection = new(conn);
                await connection.OpenAsync();

                string checkQuery = "SELECT COUNT(*) FROM accounts WHERE email = @email";

                using SqlCommand checkCmd = new(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@email", email);

#pragma warning disable CS8605 // Unboxing a possibly null value.
                int existingCount = (int)await checkCmd.ExecuteScalarAsync();
#pragma warning restore CS8605 // Unboxing a possibly null value.

                if (existingCount > 0)
                {
                    await DisplayAlertAsync("Error", "Email already registered", "OK");
                    return;
                }

                string hashedPassword = Register.HashPassword(password);

                string insertQuery = @"
                INSERT INTO accounts (email, password) 
                VALUES (@Email, @PasswordHash)";

                using SqlCommand insertCmd = new(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                int rowsAffected = await insertCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await DisplayAlertAsync("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlertAsync("Error", "Failed to create account", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlertAsync("Wrong Input", "Password confirmation is wrong!", "OK");
        }
    }
}
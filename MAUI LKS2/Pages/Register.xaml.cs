using MAUI_LKS2.Models;
using MAUI_LKS2.Services;

namespace MAUI_LKS2.Pages;

public partial class Register : ContentPage
{
    private AuthService _authService;

    public Register()
    {
        InitializeComponent();
        _authService = new AuthService();

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

    private async void OnLoginBtnClicked(object? sender, EventArgs e)
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
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string passwordConf = PasswordEntryConf.Text;

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlertAsync("Error", "Please enter an email", "OK");
            return;
        }

        if (password != passwordConf)
        {
            await DisplayAlertAsync("Wrong Input", "Password confirmation is wrong!", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlertAsync("Error", "Password must be at least 6 characters", "OK");
            return;
        }

        string role = AdminCheckBox.IsChecked ? "Admin" : "User";

        RegisterBtn.IsEnabled = false;
        RegisterBtn.Text = "Creating account...";

        try
        {
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = password,
                Role = role
            };

            bool success = await _authService.RegisterAsync(registerRequest);

            if (success)
            {
                await DisplayAlertAsync("Success", "Account created successfully!", "OK");
                await Shell.Current.GoToAsync("Pages/Login");
            }
            else
            {
                await DisplayAlertAsync("Error", "Email already registered or registration failed", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            RegisterBtn.IsEnabled = true;
            RegisterBtn.Text = "Register";
        }
    }
}
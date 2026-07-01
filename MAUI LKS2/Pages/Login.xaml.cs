using MAUI_LKS2.Services;
using MAUI_LKS2.Models;

namespace MAUI_LKS2.Pages;

public partial class Login : ContentPage
{
    private AuthService _authService;

    public Login()
    {
        InitializeComponent();

        _authService = new AuthService();

        EmailEntry.TextChanged += OnFormChange;
        PasswordEntry.TextChanged += OnFormChange;
    }

    private void OnFormChange(object? sender, EventArgs e)
    {
        bool isActive = !string.IsNullOrEmpty(EmailEntry.Text) &&
                        !string.IsNullOrEmpty(PasswordEntry.Text);

        LoginBtn.IsEnabled = isActive;
    }

    private async void OnLoginBtnClicked(object? sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlertAsync("Error", "Please enter email and password", "OK");
            return;
        }

        LoginBtn.IsEnabled = false;
        LoginBtn.Text = "Logging in...";

        try
        {
            var user = await _authService.LoginAsync(email, password);

            if (user != null)
            {
                App.CurrentUser = user;

                await DisplayAlertAsync("Success", $"Welcome!", "OK");

                await Shell.Current.GoToAsync("Pages/MainPage");
            }
            else
            {
                await DisplayAlertAsync("Login Failed", "Invalid email or password", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Login failed: {ex.Message}", "OK");
        }
        finally
        {
            LoginBtn.IsEnabled = true;
            LoginBtn.Text = "Login";
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

    private async void OnRegisterBtnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Pages/Register");
    }
}
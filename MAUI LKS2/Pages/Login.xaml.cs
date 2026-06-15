namespace MAUI_LKS2.Pages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

	private void OnLoginClicked(object? sender, EventArgs e)
	{

	}

	private void OnEyeBtnClicked(object? sender, EventArgs e)
	{
        if (PasswordEntry.IsPassword == true)
		{
			PasswordEntry.IsPassword = false;
        } else
		{
			PasswordEntry.IsPassword = true;
		}
	}

	private async void OnRegisterBtnClicked (object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Pages/Register");
	}
}
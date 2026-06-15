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

	private void OnLoginClicked(object? sender, EventArgs e)
	{
        
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
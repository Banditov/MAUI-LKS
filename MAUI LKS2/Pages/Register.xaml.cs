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
            PasswordShow.BackgroundColor = Color.FromArgb("#464646");
        }
        else
        {
            PasswordEntryConf.IsPassword = true;
            PasswordShow.BackgroundColor = Color.FromArgb("#7676ED");
        }
    }
}
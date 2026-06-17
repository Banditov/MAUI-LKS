using MAUI_LKS2.Pages;

namespace MAUI_LKS2
{
    public partial class Intro : ContentPage
    {
        public Intro()
        {
            InitializeComponent();
            // Redirect(null, EventArgs.Empty) ;
        }

        private async void Redirect(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Pages/MainPage");
        }

        private async void OnBtnClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Pages/Login");
        }
    }
}

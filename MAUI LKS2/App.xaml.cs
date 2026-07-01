using MAUI_LKS2.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MAUI_LKS2
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }

        public App()
        {
            UserAppTheme = AppTheme.Dark;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new(new AppShell())
            {
                MinimumWidth = 1300,
                MaximumWidth = 1300,
                MinimumHeight = 680,
                MaximumHeight = 680
            };

            return window;
        }
    }
}
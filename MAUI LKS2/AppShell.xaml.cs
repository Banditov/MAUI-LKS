using MAUI_LKS2.Pages;

namespace MAUI_LKS2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Pages/Login", typeof(Login));
            Routing.RegisterRoute("Pages/Register", typeof(Register));
        }
    }
}

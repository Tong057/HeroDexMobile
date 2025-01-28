using HeroDex.Views;

namespace HeroDex
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HeroPage), typeof(HeroPage));
        }
    }
}

namespace Renligou.Interface.Boss
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Renligou.Interface.Boss" };
        }
    }
}

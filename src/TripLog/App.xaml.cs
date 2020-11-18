using Ninject;
using Ninject.Modules;
using TripLog.Modules;
using TripLog.Services;
using TripLog.ViewModels;
using TripLog.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TripLog
{
    public partial class App : Application
    {
        public IKernel Kernel { get; set; }
        bool IsSignedIn = !string.IsNullOrWhiteSpace(SecureStorage.GetAsync("AccessToken").Result);

        public App(params INinjectModule[] platformModules)
        {
            InitializeComponent();

            // HACK: Setting up a placeholder MainPage for the main controller in AppDelegate
            // see: https://stackoverflow.com/questions/42150467/xamarin-ios-root-view-controller-at-the-end-of-application-launch
            MainPage = new MainPage();

            // Register core services 
            Kernel = new StandardKernel(
                new TripLogCoreModule(),
                new TripLogNavModule());

            // Register platform specific services 
            Kernel.Load(platformModules);
            var authService = Kernel.Get<IAuthService>();
            authService.AuthorizedDelegate = OnSignIn;
            SetMainPage();
        }

        void SetMainPage()
        {
            var mainPage = IsSignedIn
                ? new NavigationPage(new MainPage())
                {
                    BindingContext = Kernel.Get<MainViewModel>()
                }
                : new NavigationPage(new SignInPage())
                {
                    BindingContext = Kernel.Get<SignInViewModel>()
                };
            var navService = Kernel.Get<INavService>() as XamarinFormsNavService;
            navService.XamarinFormsNav = mainPage.Navigation;

            // HACK: Must be set on main thread to avoid exception.
            Device.BeginInvokeOnMainThread(() =>
            {
                MainPage = mainPage;
            });
        }

        void OnSignIn(string accessToken)
        {
            SecureStorage.SetAsync("AccessToken", accessToken);
            IsSignedIn = !string.IsNullOrWhiteSpace(SecureStorage.GetAsync("AccessToken").Result);
            SetMainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

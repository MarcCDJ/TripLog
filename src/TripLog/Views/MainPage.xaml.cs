using TripLog.ViewModels;
using Xamarin.Forms;

namespace TripLog.Views
{
    public partial class MainPage : ContentPage
    {
        MainViewModel ViewModel => BindingContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel?.Init();
        }
    }
}

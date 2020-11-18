using System.Threading.Tasks;
using MvvmHelpers.Commands;
using TripLog.Services;

namespace TripLog.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        readonly IAuthService _authService;

        public bool IsSignedIn { get; set; }
        public bool IsSigningIn { get; set; }
        public string Name { get; set; }

        public AsyncCommand SignInCommand { get; set; }
        public AsyncCommand SignOutCommand { get; set; }

        public SignInViewModel(INavService navService, IAuthService authService)
            : base(navService)
        {
            _authService = authService;

            SignInCommand = new AsyncCommand(SignInAsync);
            SignOutCommand = new AsyncCommand(SignOutAsync);
        }

        async Task SignInAsync()
        {
            IsSigningIn = true;

            if (await _authService.SignInAsync().ConfigureAwait(false))
            {
                IsSignedIn = true;
            }

            IsSigningIn = false;
        }

        async Task SignOutAsync()
        {
            if (await _authService.SignOutAsync().ConfigureAwait(false))
            {
                IsSignedIn = false;
            }
        }
    }
}

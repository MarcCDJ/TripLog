using System;
using System.Threading.Tasks;

namespace TripLog.Services
{
    public interface IAuthService
    {
        Action<string> AuthorizedDelegate { get; set; }
        Action<string> UnAuthorizedDelegate { get; set; }
        Task<bool> SignInAsync();
        Task<bool> SignOutAsync();
    }
}

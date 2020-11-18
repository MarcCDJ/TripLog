﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Essentials;

namespace TripLog.Services
{
    public class AuthService : IAuthService
    {
        public Action<string> AuthorizedDelegate { get; set; }

        // NOTE: Set values before building app.
        readonly string APP_ID = "app_id_value";
        readonly string CLIENT_ID = "client_id_value";
        readonly string TENANT_ID = "tenant_id_value";
        readonly string SIG_HASH = "sig_hash_value";
        private readonly string[] Scopes =
            {
                "User.Read",
                "https://mctriplog.azurewebsites.net/user_impersonation"
            };

        string RedirectUri
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    return $"msauth://{APP_ID}/{SIG_HASH}";
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    return $"msauth.{APP_ID}://auth";

                return string.Empty;
            }
        }

        /// <summary>
        /// This object is used to know where to display
        /// the authentication activity (for Android) or page.
        /// </summary>
        public static object ParentWindow { get; set; }

        IPublicClientApplication _pca;

        public AuthService()
        {
            string authority = $"https://login.microsoftonline.com/{TENANT_ID}/v2.0";
            _pca = PublicClientApplicationBuilder.Create(CLIENT_ID)
               .WithIosKeychainSecurityGroup(APP_ID)
               .WithRedirectUri(RedirectUri)
               .WithAuthority(authority)
               .Build();
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);
                var firstAccount = accounts.FirstOrDefault();
                var authResult =
                    await _pca.AcquireTokenSilent(Scopes, firstAccount)
                            .ExecuteAsync().ConfigureAwait(false);

                // Store the access token securely for later use.
                var authToken = authResult?.AccessToken;
                await SecureStorage.SetAsync("AccessToken", authToken).ConfigureAwait(false);
                AuthorizedDelegate?.Invoke(authToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult =
                        await _pca.AcquireTokenInteractive(Scopes)
                                .WithParentActivityOrWindow(ParentWindow)
                                .ExecuteAsync().ConfigureAwait(false);

                    // Store the access token securely for later use.
                    var authToken = authResult?.AccessToken;
                    await SecureStorage.SetAsync("AccessToken", authToken).ConfigureAwait(false);
                    AuthorizedDelegate?.Invoke(authToken);

                    return true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault()).ConfigureAwait(false);
                    accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
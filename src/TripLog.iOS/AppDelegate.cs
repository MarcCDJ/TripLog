
using Foundation;
using Microsoft.Identity.Client;
using TripLog.iOS.Modules;
using UIKit;

namespace TripLog.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // HACK: Setting up a placeholder controller 
            // see: https://stackoverflow.com/questions/42150467/xamarin-ios-root-view-controller-at-the-end-of-application-launch
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            window.RootViewController = new UIViewController();
            window.MakeKeyAndVisible();

            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            LoadApplication(new App(new TripLogPlatformModule()));

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Plugin.FacebookClient;

namespace Cyclon.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            var _ = new TouchTracking.Forms.iOS.TouchEffect();
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            FacebookClientManager.Initialize(app, options);

            return base.FinishedLaunching(app, options);
        }

        //Para tratar el socket de IOS
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            new CreateSocket().ConnectSocket();
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            FacebookClientManager.OnActivated();
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return FacebookClientManager.OpenUrl(app, url, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return FacebookClientManager.OpenUrl(application, url, sourceApplication, annotation);
        }

    }
}

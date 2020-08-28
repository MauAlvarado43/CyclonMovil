using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Plugin.FacebookClient;

using Plugin.GoogleClient;

using Plugin.LocalNotifications;

namespace Cyclon.Droid
{
    [Activity(Label = "Cyclon", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.CyclonMini;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //var intent = new Intent(this, typeof(CyclonSocket));
            //StartService(intent);

            FacebookClientManager.Initialize(this);
            GoogleClientManager.Initialize(this);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == 64206) {
                FacebookClientManager.OnActivityResult(requestCode, resultCode, intent);
            }
            else if (requestCode == 9637) {
                GoogleClientManager.OnAuthCompleted(requestCode, resultCode, intent);
            }  

        }

    }
}
using Cyclon.Droid;
using Cyclon.Utils;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApp))]
namespace Cyclon.Droid
{
    public class CloseApp : ICloseApp
    {
        public CloseApp() : base()
        {

        }
        public void closeApplication() {
            var activity = (Android.App.Activity) Forms.Context;
            activity.FinishAffinity();
        }
    }
}
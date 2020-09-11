using Android.Content;
using Cyclon.Droid;
using Cyclon.Utils;

using System.Diagnostics;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CreateSocket))]
namespace Cyclon.Droid {
    class CreateSocket : ISocketInitialize {
        public CreateSocket() : base()
        {
        }
        public void createSocket()
        {
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(CyclonSocket));
            Android.App.Application.Context.StartService(intent);
        }
    }
}
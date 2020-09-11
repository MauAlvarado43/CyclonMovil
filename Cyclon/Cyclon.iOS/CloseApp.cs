using Cyclon.iOS;
using Cyclon.Utils;

using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApp))]
namespace Cyclon.iOS
{
    class CloseApp : ICloseApp
    {
        public CloseApp() : base()
        {

        }

        public void closeApplication() {
            Thread.CurrentThread.Abort();
        }
    }
}
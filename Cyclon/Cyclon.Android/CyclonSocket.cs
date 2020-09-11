using Android.App;
using Android.Content;
using Android.OS;

using Plugin.LocalNotifications;

using Quobject.SocketIoClientDotNet.Client;

using Newtonsoft.Json;

using Cyclon.Resources;
using Cyclon.Utils.Objects;
using Cyclon.Utils;

using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Cyclon.Droid {

    [Service]
    class CyclonSocket : Service {

        public void ConnectSocket() {

            try {

                var socket = IO.Socket(Constants.IP);

                socket.On("/alert", async (data) => {

                    if (Preferences.Get("receiveAlerts", true)) {

                        var alert = await Task.Run(() => JsonConvert.DeserializeObject<CyclonAlert>(data.ToString()));

                        if (alert.update) {

                            double radious = alert.data.getCategoryRadious();
                            double distance = alert.data.getDistanceFromUser();

                            if (radious < distance && distance < radious + 250) {
                                CrossLocalNotifications.Current.Show(alert.data.getTitleAlert(), alert.data.getContentAlert(distance));
                            }

                        }

                    }
                    
                });

            } catch (Exception e) {

                var task = Task.Run(() => {  });

                if (task.Wait(TimeSpan.FromSeconds(10)))
                    ConnectSocket();
                else
                    throw new Exception("Timed out");

            }

        }

        public override IBinder OnBind(Intent intent) { return null; }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId){

            ConnectSocket();

            return StartCommandResult.NotSticky;
        }

    }

}
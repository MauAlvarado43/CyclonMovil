using Android.App;
using Android.Content;
using Android.OS;

using Plugin.LocalNotifications;

using Quobject.SocketIoClientDotNet.Client;

using Newtonsoft.Json;

using Cyclon.Utils.Objects;
using Cyclon.Utils;

using System;
using System.Threading.Tasks;

namespace Cyclon.Droid {

    [Service]
    class CyclonSocket : Service {

        public void ConnectSocket() {

            try {

                var socket = IO.Socket(Constants.IP);

                socket.On("/alert", (data) => {
                    Cyclone cyclone = JsonConvert.DeserializeObject<Cyclone>(data.ToString());

                    CrossLocalNotifications.Current.Show("dsadsasdas", "dsadasd");
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

            //ConnectSocket();

            return StartCommandResult.NotSticky;
        }

    }

}
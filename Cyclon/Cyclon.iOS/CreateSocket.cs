using Cyclon.iOS;
using Cyclon.Utils;
using Cyclon.Utils.Objects;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(CreateSocket))]
namespace Cyclon.iOS {
    class CreateSocket {
        public CreateSocket() : base()
        {
            ConnectSocket();
        }

        public void ConnectSocket()
        {
            try {

                var socket = IO.Socket(Constants.IP);

                socket.On("/alert", async (data) => {

                    if (Preferences.Get("receiveAlerts", true))
                    {

                        var alert = await Task.Run(() => JsonConvert.DeserializeObject<CyclonAlert>(data.ToString()));

                        if (alert.update)
                        {

                            double radious = alert.data.getCategoryRadious();
                            double distance = alert.data.getDistanceFromUser();

                            if (radious < distance && distance < radious + 250)
                            {
                                CrossLocalNotifications.Current.Show(alert.data.getTitleAlert(), alert.data.getContentAlert(distance));
                            }

                        }

                    }

                });

            }
            catch (Exception e)
            {

                var task = Task.Run(() => { });

                if (task.Wait(TimeSpan.FromSeconds(10)))
                    ConnectSocket();
                else
                    throw new Exception("Timed out");

            }
        }
    }
}
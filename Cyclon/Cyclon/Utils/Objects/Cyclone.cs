using Cyclon.Resources;
using System;
using Xamarin.Essentials;

namespace Cyclon.Utils.Objects {
    public class Cyclone {
        public string id { get; set; }
        public string name { get; set; }

        public string category { get; set; }
        public Geo location { get; set; }
        public double getDistanceFromUser() {

            double R = 6378.137;
            double latUser = Double.Parse(Cipher.decrypt(Preferences.Get("lat", "")));
            double lngUser = Double.Parse(Cipher.decrypt(Preferences.Get("lng", "")));

            double dLat = Math.Abs((this.location.lat - latUser) * Math.PI / 180);
            double dLng = Math.Abs((this.location.lng - lngUser) * Math.PI / 180);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(this.location.lat * Math.PI / 180) * Math.Cos(latUser * Math.PI / 180) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return Math.Round((R * c) * 100d) / 100d;

        }
        public double getCategoryRadious() {
            if (this.category == "DT") return 75;
            else if (this.category == "TT") return 100;
            else if(this.category == "H1") return 300;
            else if(this.category == "H2") return 700;
            else if(this.category == "H3") return 900;
            else if (this.category == "H4") return 1100;
            else return 1300;
        }
        public string getTitleAlert() {
            return AppResources.alert;
        }
        public string getContentAlert(double distance)
        {
            return AppResources.contentAlert.Replace("$[0]", getCategoryString()).Replace("$[1]", this.name).Replace("$[2]", distance + "");
        }
        public string getCategoryString() {
            if (this.category == "DT") return AppResources.dt;
            else if (this.category == "TT") return AppResources.tt;
            else return AppResources.h;
        }

    }
}

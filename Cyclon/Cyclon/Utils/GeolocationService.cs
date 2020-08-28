using System;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Cyclon.Utils.Objects;

namespace Cyclon.Utils {
    class GeolocationService {
        public static async Task<Geo> getLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                Geo position = new Geo();
                position.lat = location.Latitude;
                position.lng = location.Longitude;

                return position;

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                return null;
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                return null;
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                return null;
                // Handle permission exception
            }
            catch (Exception ex)
            {
                return null;
                // Unable to get location
            }
        }
    }
}

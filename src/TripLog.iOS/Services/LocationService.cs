using System.Threading.Tasks;
using TripLog.Models;
using TripLog.Services;

namespace TripLog.iOS.Services
{
    public class LocationService : ILocationService
    {
        public LocationService()
        {
        }

        public async Task<GeoCoords> GetGeoCoordinatesAsync()
        {
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync();
            return new GeoCoords
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}

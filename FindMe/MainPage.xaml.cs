using System.Threading.Tasks;

namespace FindMe
{
    public partial class MainPage : ContentPage
    {
        readonly string baseUrl = "https://bing.com/maps/default.aspx?cp=";

        public string UserName { get; set; } = default!;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFindMeClicked(object? sender, EventArgs e)
        {
            PermissionStatus permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permissions == PermissionStatus.Granted)
            {
                await ShareLocation();
            }
            else
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Permissions Error",
                    "You have not granted the app permission to access your location.", "OK");
                    PermissionStatus requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (requested == PermissionStatus.Granted)
                    {
                        await ShareLocation();
                    }
                    else
                    {
                        if (DeviceInfo.Platform == DevicePlatform.iOS || 
                            DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                        {
                            await Shell.Current.DisplayAlert("Location Required", 
                                "Location is required to share it. Please enable location for this app in Settings.", "OK");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Location Required",
                                "Location is required to share it. We will ask again next time.", "OK");
                        }
                    }
                }
            }
        }

        private async Task ShareLocation()
        {
            UserName = UsernameEntry.Text;

            GeolocationRequest locationRequest = new (GeolocationAccuracy.Best);
            Location? location = await Geolocation.GetLocationAsync(locationRequest);
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = "Find me!",
                Title = "Find me!",
                Text = $"{UserName} is sharing their location with you",
                Uri = $"{baseUrl}{location?.Latitude}~{location?.Longitude}"
            });
        }
    }
}

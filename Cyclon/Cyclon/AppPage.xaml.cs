using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

using Cyclon.Utils;
using System.Diagnostics;

namespace Cyclon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : Xamarin.Forms.TabbedPage
    {
        public AppPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Default);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);

            alerts.IsToggled = Preferences.Get("receiveAlerts", true);

            Debug.WriteLine("SOCKEEEEEEEEEEEEEET");

            var socket = DependencyService.Get<ISocketInitialize>();
            socket.createSocket();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public async void logout(object sender, EventArgs e)
        {
            Preferences.Clear();
            await Navigation.PushAsync(new MainPage());
        }

        public async void update(object sender, EventArgs e) {
            await Navigation.PushAsync(new UpdateData());
        }

        public void contact(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("mailto:hypersoft@gmail.com"));
        }

        public void changeAlerts(object sender, EventArgs e) {
            Preferences.Set("receiveAlerts", alerts.IsToggled);
        }

    }

}
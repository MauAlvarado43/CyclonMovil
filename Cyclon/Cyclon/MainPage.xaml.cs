using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using Cyclon.Utils;

namespace Cyclon {

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage {

        public MainPage(){

            InitializeComponent();
            
            string email = Preferences.Get("email", "");

            if (email != "") {
                Task.Run(async () => { await Navigation.PushAsync(new AppPage()); });
            }

            labelLogin.GestureRecognizers.Add( new TapGestureRecognizer() {
                Command = new Command( async () => {
                    await Navigation.PushAsync(new LoginPage());
                })
            });

        }

        public async void toRegister(object sender, EventArgs e){
            await Navigation.PushAsync(new RegisterPage());
        }

        protected override bool OnBackButtonPressed()
        {
            var closer = DependencyService.Get<ICloseApp>();
            closer.closeApplication();

            return true;
        }

    }
}

using System;
using System.ComponentModel;

using System.Globalization;
using Xamarin.Forms;

namespace Cyclon {

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage {
        public MainPage(){

            InitializeComponent();

            String a = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            Console.WriteLine(a);

            labelLogin.GestureRecognizers.Add( new TapGestureRecognizer() {
                Command = new Command( async () => {
                    await Navigation.PushAsync(new LoginPage());
                })
            });

        }

        public async void toRegister(object sender, EventArgs e){
            await Navigation.PushAsync(new RegisterPage());
        }

    }
}

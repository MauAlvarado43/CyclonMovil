using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;

using Xamarin.Forms;

using Cyclon.Utils;
using Cyclon.Resources;
using Cyclon.Utils.Objects;
using Acr.UserDialogs;
using Xamarin.Essentials;

namespace Cyclon
{
    [DesignTimeVisible(false)]
    public partial class RegisterPage : ContentPage {
        public RegisterPage() {
            InitializeComponent();
        }

        public async void register(object sender, EventArgs e) {

            bool flag = true;

            /* Data Validation*/

            nameError.Text = "";
            lastNameError.Text = "";
            emailError.Text = "";
            passwordError.Text = "";

            if (name.Text ==null || !RegexValidation.checkWords(name.Text)) { nameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
            else if (name.Text.Length == 0) { nameError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (name.Text.Length > 50) { nameError.Text = AppResources.DATA_LONG; flag = false; }

            if (lastName.Text == null || !RegexValidation.checkWords(lastName.Text)) { lastNameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
            else if (lastName.Text.Length == 0) { lastNameError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (lastName.Text.Length > 50) { lastNameError.Text = AppResources.DATA_LONG; flag = false; }

            if (email.Text == null || !RegexValidation.checkEmail(email.Text)) { emailError.Text = AppResources.EMAIL_FORMAT; flag = false; }
            else if (email.Text.Length == 0) { emailError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (email.Text.Length > 50) { emailError.Text = AppResources.DATA_LONG; flag = false; }

            if (password.Text == null || password.Text.Length == 0) { passwordError.Text = AppResources.PASSWORD_EMPTY; flag = false; }
            else if (password.Text.Length < 8) { passwordError.Text = AppResources.PASSWORD_SHORT; flag = false; }
            else if (password.Text.Length > 50) { passwordError.Text = AppResources.PASSWORD_LONG; flag = false; }
            else if (password.Text != cpassword.Text) { passwordError.Text = AppResources.PASSWORD_DONT_MATCH; flag = false; }

            if (flag) {

                CyclonProfile profile = new CyclonProfile();
                profile.email = email.Text;
                profile.lastName = lastName.Text;
                profile.location = await GeolocationService.getLocation();
                profile.name = name.Text;
                profile.password = password.Text;
                profile.photo = null;

                UserDialogs.Instance.ShowLoading(AppResources.loading);

                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("lastName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("name", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("password", Cipher.encrypt(profile.password)),
                        new KeyValuePair<string, string>("lng", Cipher.encrypt(profile.location.lng.ToString())),
                        new KeyValuePair<string, string>("lat", Cipher.encrypt(profile.location.lat.ToString()))
                    }
                );

                var response = await RestService.POST(content, "/auth/mobile/register");

                UserDialogs.Instance.HideLoading();

                if (response == "{\"code\":200,\"msg\":\"SIGNUP_SUCCESS\"}") {
                    var toastConfig = new ToastConfig(AppResources.ACCOUNT_CREATED_SUCCESS);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                    UserDialogs.Instance.Toast(toastConfig);

                    Preferences.Set("email", Cipher.encrypt(profile.email));
                    Preferences.Set("lastName", Cipher.encrypt(profile.lastName));
                    Preferences.Set("name", Cipher.encrypt(profile.name));
                    Preferences.Set("password", Cipher.encrypt(profile.password));
                    Preferences.Set("lng", Cipher.encrypt(profile.location.lng.ToString()));
                    Preferences.Set("lat", Cipher.encrypt(profile.location.lat.ToString()));
                    Preferences.Set("typeLogin", "/auth/mobile/login");
                    Preferences.Set("receiveAlerts", true);

                    await Navigation.PushAsync(new AppPage());

                }
                else if (response == "{\"code\":401,\"msg\":[\"EMAIL_TAKEN\"]}") {
                    var toastConfig = new ToastConfig(AppResources.EMAIL_REGISTERED);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }
                else if (response == "{\"code\":401,\"msg\":[\"BAD_LOCATION\"]}") {
                    var toastConfig = new ToastConfig(AppResources.IP_NOT_FOUND);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }
                else {
                    var toastConfig = new ToastConfig(AppResources._500);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }

            }

        }

    }
}
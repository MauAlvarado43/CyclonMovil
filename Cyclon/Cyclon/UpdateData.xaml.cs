using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Cyclon.Utils;
using System.Net.Http;
using Acr.UserDialogs;
using Cyclon.Resources;
using System.Diagnostics;

namespace Cyclon {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateData : ContentPage {

        string type = Preferences.Get("typeLogin", "");

        public UpdateData() {
            InitializeComponent();

            if (type == "") {
                Task.Run(async () => { await Navigation.PushAsync(new AppPage()); });
            }
            else if (type != "/auth/mobile/login") {
                containerEmail.IsVisible = false;
                containerPassword.IsVisible = false;
            }

            name.Text = Cipher.decrypt(Preferences.Get("name", ""));
            lastName.Text = Cipher.decrypt(Preferences.Get("lastName", ""));
            email.Text = Cipher.decrypt(Preferences.Get("email", ""));

        }


        public async void updateLocation(object sender, EventArgs e) {

            UserDialogs.Instance.ShowLoading(AppResources.loading);

            var geo = await GeolocationService.getLocation();

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("lng", Cipher.encrypt(geo.lng.ToString())),
                    new KeyValuePair<string, string>("lat", Cipher.encrypt(geo.lat.ToString())),
                    new KeyValuePair<string, string>("email", Preferences.Get("email","")),
                }
            );

            var response = await RestService.POST(content, "/api/mobile/updateLocation");

            UserDialogs.Instance.HideLoading();

            if (response == "{\"code\":200,\"msg\":\"SUCCESS\"}") {

                Preferences.Set("lng", Cipher.encrypt(geo.lng.ToString()));
                Preferences.Set("lat", Cipher.encrypt(geo.lat.ToString()));

                var toastConfig = new ToastConfig(AppResources.SUCCESS_LOCATION);

                toastConfig.SetDuration(2000);
                toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                UserDialogs.Instance.Toast(toastConfig);
            }
            else {
                var toastConfig = new ToastConfig(AppResources._500);

                toastConfig.SetDuration(2000);
                toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                UserDialogs.Instance.Toast(toastConfig);
            }

        }

        public async void updatePassword(object sender, EventArgs e) {
            if (type == "/auth/mobile/login") {

                bool flag = true;

                if (password.Text == null || password.Text.Length == 0) { passwordError.Text = AppResources.PASSWORD_EMPTY; flag = false; }
                else if (password.Text.Length < 8) { passwordError.Text = AppResources.PASSWORD_SHORT; flag = false; }
                else if (password.Text.Length > 50) { passwordError.Text = AppResources.PASSWORD_LONG; flag = false; }
                else if (password.Text != cpassword.Text) { passwordError.Text = AppResources.PASSWORD_DONT_MATCH; flag = false; }

                if (flag) {

                    PromptConfig prompt = new PromptConfig();
                    prompt.Title = AppResources.PASSWORD_VERIFY;
                    prompt.IsCancellable = false;
                    prompt.SetInputMode(InputType.Password);

                    PromptResult result = await UserDialogs.Instance.PromptAsync(prompt);

                    if (result.Ok && !string.IsNullOrWhiteSpace(result.Text)) {

                        UserDialogs.Instance.ShowLoading(AppResources.loading);

                        FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("email", Preferences.Get("email", "")),
                                new KeyValuePair<string, string>("password", Cipher.encrypt(password.Text))
                            }
                        );

                        UserDialogs.Instance.HideLoading();

                        var response = await RestService.POST(content, "/api/mobile/updatePassword");

                        if (response == "{\"code\":200,\"msg\":\"SUCCESS\"}") {

                            var toastConfig = new ToastConfig(AppResources.PASSWORD_UPDATED);

                            toastConfig.SetDuration(2000);
                            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                            UserDialogs.Instance.Toast(toastConfig);

                            Preferences.Clear();

                            await Navigation.PushAsync(new MainPage());

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

        public async void update(object sender, EventArgs e)
        {

            if (type == "") {
                Task.Run(async () => { await Navigation.PushAsync(new AppPage()); });
            }
            else if (type == "/auth/mobile/login") {

                bool flag = true;

                if (name.Text == null || !RegexValidation.checkWords(name.Text)) { nameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
                else if (name.Text.Length == 0) { nameError.Text = AppResources.DATA_EMPTY; flag = false; }
                else if (name.Text.Length > 50) { nameError.Text = AppResources.DATA_LONG; flag = false; }

                if (lastName.Text == null || !RegexValidation.checkWords(lastName.Text)) { lastNameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
                else if (lastName.Text.Length == 0) { lastNameError.Text = AppResources.DATA_EMPTY; flag = false; }
                else if (lastName.Text.Length > 50) { lastNameError.Text = AppResources.DATA_LONG; flag = false; }

                if (email.Text == null || !RegexValidation.checkEmail(email.Text)) { emailError.Text = AppResources.EMAIL_FORMAT; flag = false; }
                else if (email.Text.Length == 0) { emailError.Text = AppResources.DATA_EMPTY; flag = false; }
                else if (email.Text.Length > 50) { emailError.Text = AppResources.DATA_LONG; flag = false; }

                if (flag) {

                    if (email.Text == Cipher.decrypt(Preferences.Get("email", ""))) {

                        UserDialogs.Instance.ShowLoading(AppResources.loading);

                        FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("name", Cipher.encrypt(name.Text)),
                                new KeyValuePair<string, string>("lastName", Cipher.encrypt(lastName.Text)),
                                new KeyValuePair<string, string>("actualEmail", Preferences.Get("email", ""))
                            }
                        );

                        UserDialogs.Instance.HideLoading();

                        var response = await RestService.POST(content, "/api/mobile/updateInfo");

                        if (response == "{\"code\":200,\"msg\":\"SUCCESS\"}") {

                            var toastConfig = new ToastConfig(AppResources.INFO_UPDATED);

                            toastConfig.SetDuration(2000);
                            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                            UserDialogs.Instance.Toast(toastConfig);

                            Preferences.Set("name", Cipher.encrypt(name.Text));
                            Preferences.Set("lastName", Cipher.encrypt(lastName.Text));

                        }
                        else {
                            var toastConfig = new ToastConfig(AppResources._500);

                            toastConfig.SetDuration(2000);
                            toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                            UserDialogs.Instance.Toast(toastConfig);
                        }
                    }
                    else {

                        PromptConfig prompt = new PromptConfig();
                        prompt.Title = AppResources.PASSWORD_VERIFY;
                        prompt.IsCancellable = false;
                        prompt.SetInputMode(InputType.Password);

                        PromptResult result = await UserDialogs.Instance.PromptAsync(prompt);

                        if (result.Ok && !string.IsNullOrWhiteSpace(result.Text)) {

                            if (Cipher.decrypt(Preferences.Get("password", "")) == result.Text) {

                                UserDialogs.Instance.ShowLoading(AppResources.loading);

                                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                                        new KeyValuePair<string, string>("name", Cipher.encrypt(name.Text)),
                                        new KeyValuePair<string, string>("lastName", Cipher.encrypt(lastName.Text)),
                                        new KeyValuePair<string, string>("email", Cipher.encrypt(email.Text)),
                                        new KeyValuePair<string, string>("actualEmail", Preferences.Get("email", ""))
                                    }
                                );

                                var response = await RestService.POST(content, "/api/mobile/updateAllInfo");

                                UserDialogs.Instance.HideLoading();

                                if (response == "{\"code\":200,\"msg\":\"SUCCESS\"}") {

                                    var toastConfig = new ToastConfig(AppResources.INFO_UPDATED_RELOGIN);

                                    toastConfig.SetDuration(2000);
                                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                                    UserDialogs.Instance.Toast(toastConfig);

                                    Preferences.Clear();

                                    await Navigation.PushAsync(new MainPage());

                                }
                                else {
                                    var toastConfig = new ToastConfig(AppResources.EMAIL_VERIFY);

                                    toastConfig.SetDuration(2000);
                                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                                    UserDialogs.Instance.Toast(toastConfig);
                                }

                            }
                            else {
                                var toastConfig = new ToastConfig(AppResources.PASSWORD_INCORRECT);

                                toastConfig.SetDuration(2000);
                                toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                                UserDialogs.Instance.Toast(toastConfig);
                            }
                        }
                    }

                }

            }
            else {

                bool flag = true;

                if (name.Text == null || !RegexValidation.checkWords(name.Text)) { nameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
                else if (name.Text.Length == 0) { nameError.Text = AppResources.DATA_EMPTY; flag = false; }
                else if (name.Text.Length > 50) { nameError.Text = AppResources.DATA_LONG; flag = false; }

                if (lastName.Text == null || !RegexValidation.checkWords(lastName.Text)) { lastNameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
                else if (lastName.Text.Length == 0) { lastNameError.Text = AppResources.DATA_EMPTY; flag = false; }
                else if (lastName.Text.Length > 50) { lastNameError.Text = AppResources.DATA_LONG; flag = false; }

                if (flag) {

                    UserDialogs.Instance.ShowLoading(AppResources.loading);

                    FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                            new KeyValuePair<string, string>("name", Cipher.encrypt(name.Text)),
                            new KeyValuePair<string, string>("lastName", Cipher.encrypt(lastName.Text)),
                            new KeyValuePair<string, string>("actualEmail", Preferences.Get("email", ""))
                        }
                    );

                    var response = await RestService.POST(content, "/api/mobile/updateInfo");

                    UserDialogs.Instance.HideLoading();

                    if (response == "{\"code\":200,\"msg\":\"SUCCESS\"}") {

                        var toastConfig = new ToastConfig(AppResources.INFO_UPDATED);

                        toastConfig.SetDuration(2000);
                        toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                        UserDialogs.Instance.Toast(toastConfig);

                        Preferences.Set("name", Cipher.encrypt(name.Text));
                        Preferences.Set("lastName", Cipher.encrypt(lastName.Text));

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
}
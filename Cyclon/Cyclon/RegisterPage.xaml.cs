using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;

using Xamarin.Forms;

using Cyclon.Utils;
using Cyclon.Resources;
using Cyclon.Utils.Objects;

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

            nameError.IsVisible = false; nameError.Text = "";
            lastNameError.IsVisible = false; lastNameError.Text = "";
            emailError.IsVisible = false; emailError.Text = "";
            passwordError.IsVisible = false; passwordError.Text = "";

            if (name.Text ==null || !RegexValidation.checkWords(name.Text)) { nameError.IsVisible = true; nameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
            else if (name.Text.Length == 0) { nameError.IsVisible = true; nameError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (name.Text.Length > 50) { nameError.IsVisible = true; nameError.Text = AppResources.DATA_LONG; flag = false; }

            if (lastName.Text == null || !RegexValidation.checkWords(lastName.Text)) { lastNameError.IsVisible = true; lastNameError.Text = AppResources.DATA_SPACES_AND_LETTERS; flag = false; }
            else if (lastName.Text.Length == 0) { lastNameError.IsVisible = true; lastNameError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (lastName.Text.Length > 50) { lastNameError.IsVisible = true; lastNameError.Text = AppResources.DATA_LONG; flag = false; }

            if (email.Text == null || !RegexValidation.checkEmail(email.Text)) { emailError.IsVisible = true; emailError.Text = AppResources.EMAIL_FORMAT; flag = false; }
            else if (email.Text.Length == 0) { emailError.IsVisible = true; emailError.Text = AppResources.DATA_EMPTY; flag = false; }
            else if (email.Text.Length > 50) { emailError.IsVisible = true; emailError.Text = AppResources.DATA_LONG; flag = false; }

            if (password.Text == null || password.Text.Length == 0) { passwordError.IsVisible = true; passwordError.Text = AppResources.PASSWORD_EMPTY; flag = false; }
            else if (password.Text.Length < 8) { passwordError.IsVisible = true; passwordError.Text = AppResources.PASSWORD_SHORT; flag = false; }
            else if (password.Text.Length > 50) { passwordError.IsVisible = true; passwordError.Text = AppResources.PASSWORD_LONG; flag = false; }
            else if (password.Text != cpassword.Text) { passwordError.IsVisible = true; passwordError.Text = AppResources.PASSWORD_DONT_MATCH; flag = false; }

            if (flag) {

                CyclonProfile profile = new CyclonProfile();
                profile.email = email.Text;
                profile.lastName = lastName.Text;
                profile.location = await GeolocationService.getLocation();
                profile.name = name.Text;
                profile.password = password.Text;
                profile.photo = null;

                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("lastName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("name", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("password", Cipher.encrypt(profile.password))
                    }
                );

                var response = await RestService.POST(content, "/auth/mobile/register");
                Debug.WriteLine(response);

            }

        }

    }
}
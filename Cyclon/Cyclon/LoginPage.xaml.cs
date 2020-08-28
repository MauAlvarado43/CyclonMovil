using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

using Xamarin.Forms;

using Newtonsoft.Json;

using Plugin.FacebookClient;

using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;

using Cyclon.Utils.Objects;
using Cyclon.Utils;

namespace Cyclon {

    [DesignTimeVisible(false)]
    public partial class LoginPage : ContentPage {

        IFacebookClient _facebookService = CrossFacebookClient.Current;
        IGoogleClientManager _googleService = CrossGoogleClient.Current;

        public LoginPage() {

            InitializeComponent();
        }

        public async void login(object sender, EventArgs e) {

            CyclonProfile profile = new CyclonProfile();
            profile.email = email.Text;
            profile.password = password.Text;
            profile.location = await GeolocationService.getLocation();
            profile.photo = null;
            profile.lastName = null;
            profile.name = null;


            loginRequest(profile, "/auth/login");
        }

        public async void loginFacebook(object senderx, EventArgs ex) {
            try {

                if (_facebookService.IsLoggedIn) _facebookService.Logout();

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) => {
                    if (e == null) return;

                    switch (e.Status) {
                        case FacebookActionStatus.Completed:

                            var facebookUserString = JsonConvert.SerializeObject(e.Data);
                            var facebookProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookProfile>(e.Data));
                            Debug.WriteLine(facebookProfile.picture_large.data.url);

                            CyclonProfile profile = new CyclonProfile();
                            profile.email = facebookProfile.email;
                            profile.lastName = facebookProfile.last_name;
                            profile.location = await GeolocationService.getLocation();
                            profile.name = facebookProfile.first_name;
                            profile.password = facebookProfile.id;
                            profile.photo = facebookProfile.picture_large.data.url;

                            loginRequest(profile, "/auth/mobile/facebook");

                            break;
                        case FacebookActionStatus.Canceled:
                            break;
                    }

                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "gender", "last_name", "id", "picture.width(720).height(720).as(picture_large)" };
                string[] fbPermisions = { "email" };
                await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);
            } catch (Exception exx) {
                Debug.WriteLine(exx.ToString());
            }
        }

        public async void loginGoogle(object senderx, EventArgs ex) {
            try {

                _googleService.Logout();

                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = async (object sender, GoogleClientResultEventArgs<GoogleUser> e) => {
                    switch (e.Status) {
                        case GoogleActionStatus.Completed:

                            var googleUserString = JsonConvert.SerializeObject(e.Data);

                            var googleProfile = new GoogleProfile();
                            googleProfile.Email = e.Data.Email;
                            googleProfile.FamilyName = e.Data.FamilyName;
                            googleProfile.GivenName = e.Data.GivenName;
                            googleProfile.Id = e.Data.Id;
                            googleProfile.Picture = e.Data.Picture.ToString();

                            CyclonProfile profile = new CyclonProfile();
                            profile.email = googleProfile.Email;
                            profile.lastName = googleProfile.FamilyName;
                            profile.location = await GeolocationService.getLocation();
                            profile.name = googleProfile.GivenName;
                            profile.password = googleProfile.Id;
                            profile.photo = googleProfile.Picture;

                            loginRequest(profile, "/auth/mobile/google");

                            break;
                        case GoogleActionStatus.Canceled:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Canceled", "Ok");
                            break;
                        case GoogleActionStatus.Error:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Error", "Ok");
                            break;
                        case GoogleActionStatus.Unauthorized:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Unauthorized", "Ok");
                            break;
                    }

                    _googleService.OnLogin -= userLoginDelegate;
                };

                _googleService.OnLogin += userLoginDelegate;

                await _googleService.LoginAsync();
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.ToString());
            }
        }

        public async void loginRequest(CyclonProfile profile, string URL_TARGET) {

            FormUrlEncodedContent content = null;

            if (URL_TARGET == "/auth/mobile/facebook")
            {
                content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("lastName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("name", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("id", Cipher.encrypt( profile.password))
                    }
                );
            }
            else if (URL_TARGET == "/auth/mobile/google")
            {
                content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("familyName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("givenName", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("id", Cipher.encrypt(profile.password))
                    }
                );
            }
            else {
                content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("password", Cipher.encrypt(profile.password))
                    }
                );
            }

            var response = await RestService.POST(content, URL_TARGET);
            Debug.WriteLine(response);

        }

    }
}
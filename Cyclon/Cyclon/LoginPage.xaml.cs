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
using Cyclon.Resources;

using Acr.UserDialogs;

using Xamarin.Essentials;

namespace Cyclon {

    [DesignTimeVisible(false)]
    public partial class LoginPage : ContentPage {

        IFacebookClient _facebookService = CrossFacebookClient.Current;
        IGoogleClientManager _googleService = CrossGoogleClient.Current;

        public LoginPage() {
            InitializeComponent();
        }

        public void googleBtnAction(object sender, TouchTracking.TouchActionEventArgs args)
        {
            try
            {
                if (args.Type == TouchTracking.TouchActionType.Pressed)
                {
                    loginGoogleBtn.BackgroundColor = Color.FromHex("1e1e1e");
                    labelGoogleBtn.TextColor = Color.FromHex("ffffff");
                }
                else
                {
                    loginGoogleBtn.BackgroundColor = Color.FromHex("ffffff");
                    labelGoogleBtn.TextColor = Color.FromHex("000000");
                    loginGoogle();
                }
            }
            catch (Exception e)
            {
            }
        }

        public void facebookBtnAction(object sender, TouchTracking.TouchActionEventArgs args) {

            try {
                if (args.Type == TouchTracking.TouchActionType.Pressed)
                {
                    loginFacebookBtn.BackgroundColor = Color.FromHex("236bc9");
                }
                else
                {
                    loginFacebookBtn.BackgroundColor = Color.FromHex("1977f3");
                    loginFacebook();
                }
            }
            catch (Exception e) { 
            }
        }

        public async void login(object sender, EventArgs e) {

            CyclonProfile profile = new CyclonProfile();
            profile.email = email.Text;
            profile.password = password.Text;
            profile.location = await GeolocationService.getLocation();
            profile.photo = null;
            profile.lastName = null;
            profile.name = null;

            loginRequest(profile, "/auth/mobile/login");
        }

        public async void loginFacebook() {

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

        public async void loginGoogle() {
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
                            
                            break;
                        case GoogleActionStatus.Error:
                            
                            break;
                        case GoogleActionStatus.Unauthorized:
                            
                            break;
                        default:
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

            try {
                UserDialogs.Instance.ShowLoading(AppResources.loading);

                FormUrlEncodedContent content = null;

                if (URL_TARGET == "/auth/mobile/facebook")
                {
                    content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("lastName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("name", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("id", Cipher.encrypt( profile.password)),
                        new KeyValuePair<string, string>("lng", Cipher.encrypt(profile.location.lng.ToString())),
                        new KeyValuePair<string, string>("lat", Cipher.encrypt(profile.location.lat.ToString()))
                    }
                    );
                }
                else if (URL_TARGET == "/auth/mobile/google")
                {
                    content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("familyName", Cipher.encrypt(profile.lastName)),
                        new KeyValuePair<string, string>("givenName", Cipher.encrypt(profile.name)),
                        new KeyValuePair<string, string>("id", Cipher.encrypt(profile.password)),
                        new KeyValuePair<string, string>("lng", Cipher.encrypt(profile.location.lng.ToString())),
                        new KeyValuePair<string, string>("lat", Cipher.encrypt(profile.location.lat.ToString()))
                    }
                    );
                }
                else
                {
                    content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)),
                        new KeyValuePair<string, string>("password", Cipher.encrypt(profile.password))
                    }
                    );
                }

                var response = await RestService.POST(content, URL_TARGET);

                UserDialogs.Instance.HideLoading();

                if (response == "{\"code\":200,\"msg\":\"LOGIN_SUCCESS\"}")
                {

                    var responsex = await RestService.POST(new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)) }), "/api/mobile/getUser");

                    var toastConfig = new ToastConfig(AppResources.LOG_IN);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                    UserDialogs.Instance.Toast(toastConfig);

                    Preferences.Set("email", Cipher.encrypt(profile.email));
                    Preferences.Set("lastName", Cipher.encrypt(responsex.Split('/')[1]));
                    Preferences.Set("name", Cipher.encrypt(responsex.Split('/')[0]));
                    Preferences.Set("password", Cipher.encrypt(profile.password));
                    Preferences.Set("lng", Cipher.encrypt(profile.location.lng.ToString()));
                    Preferences.Set("lat", Cipher.encrypt(profile.location.lat.ToString()));
                    Preferences.Set("typeLogin", URL_TARGET);
                    Preferences.Set("receiveAlerts", true);

                    await Navigation.PushAsync(new AppPage());
                }
                else if (response == "{\"code\":200,\"msg\":[\"LOGIN_SUCCESS\"]}")
                {

                    var responsex = await RestService.POST(new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("email", Cipher.encrypt(profile.email)) }), "/api/mobile/getUser");

                    var toastConfig = new ToastConfig(AppResources.LOG_IN);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(78, 177, 62));

                    UserDialogs.Instance.Toast(toastConfig);

                    Preferences.Set("email", Cipher.encrypt(profile.email));
                    Preferences.Set("lastName", Cipher.encrypt(responsex.Split('/')[1]));
                    Preferences.Set("name", Cipher.encrypt(responsex.Split('/')[0]));
                    Preferences.Set("password", Cipher.encrypt(profile.password));
                    Preferences.Set("lng", Cipher.encrypt(profile.location.lng.ToString()));
                    Preferences.Set("lat", Cipher.encrypt(profile.location.lat.ToString()));
                    Preferences.Set("typeLogin", URL_TARGET);
                    Preferences.Set("receiveAlerts", true);

                    await Navigation.PushAsync(new AppPage());
                }
                else if (response == "{\"code\":200,\"msg\":\"ACCOUNT_CREATED\"}")
                {
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
                    Preferences.Set("typeLogin", URL_TARGET);
                    Preferences.Set("receiveAlerts", true);

                    await Navigation.PushAsync(new AppPage());
                }
                else if (response == "{\"code\":401,\"msg\":[\"USER_NOT_EXIST\"]}")
                {
                    var toastConfig = new ToastConfig(AppResources.USER_NOT_EXIST);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }
                else if (response == "{\"code\":401,\"msg\":[\"INCORRECT_PASSWORD\"]}")
                {
                    var toastConfig = new ToastConfig(AppResources.PASSWORD_INCORRECT);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }
                else
                {
                    var toastConfig = new ToastConfig(AppResources._500);

                    toastConfig.SetDuration(2000);
                    toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(210, 49, 40));

                    UserDialogs.Instance.Toast(toastConfig);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

    }
}
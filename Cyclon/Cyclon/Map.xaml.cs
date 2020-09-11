using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;

using Cyclon.Utils;
using Cyclon.Resources;

namespace Cyclon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Map : ContentPage
    {
        public Map()
        {
            InitializeComponent();

            errorPage.IsVisible = false;

            web.IsVisible = false;
            web.Source = Constants.IP + "/mapMobile?lat=" + Cipher.decrypt(Preferences.Get("lat", "19")) + "&lng=" + Cipher.decrypt(Preferences.Get("lng", "-99"));
            web.Navigated += OnWebViewNavigated;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            web.Reload();
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            switch (e.Result)
            {
                case WebNavigationResult.Cancel:

                    if (AppResources.location == "es")
                    {
                        errorPage.Source = "Images/_404_es.png";
                    }
                    else
                    {
                        errorPage.Source = "Images/_404_en.png";
                    }
                    errorPage.IsVisible = true;
                    web.IsVisible = false;

                    break;
                case WebNavigationResult.Failure:

                    if (AppResources.location == "es")
                    {
                        errorPage.Source = "Images/_404_es.png";
                    }
                    else
                    {
                        errorPage.Source = "Images/_404_en.png";
                    }
                    errorPage.IsVisible = true;
                    web.IsVisible = false;

                    break;
                case WebNavigationResult.Success:

                    errorPage.IsVisible = false;
                    web.IsVisible = true;

                    break;
                case WebNavigationResult.Timeout:

                    if (AppResources.location == "es")
                    {
                        errorPage.Source = "Images/_404_es.png";
                    }
                    else
                    {
                        errorPage.Source = "Images/_404_en.png";
                    }
                    errorPage.IsVisible = true;
                    web.IsVisible = false;

                    break;
                default:
                    break;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }

}
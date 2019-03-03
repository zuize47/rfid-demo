using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Platform;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace BLE.Client.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity
        : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate 
            {
                return true;
            };

            /*System.Net.ServicePointManager
                .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
                */

            base.OnCreate(bundle);

            //Plugin.CurrentActivity.CrossCurrentActivity.Current.Init();
            //Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            UserDialogs.Init(this);
            Forms.Init(this, bundle);
            var formsApp = new BleMvxFormsApp();
            LoadApplication(formsApp);

            var presenter = (MvxFormsDroidPagePresenter) Mvx.Resolve<IMvxViewPresenter>();
            presenter.MvxFormsApp = formsApp;

            Mvx.Resolve<IMvxAppStart>().Start();
        }
    }
}
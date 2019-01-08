using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Acr.UserDialogs;
using Explayer.Droid.Services;

namespace Explayer.Droid
{
    [Activity(Label = "Explayer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // Initialize services
            HandleStaticFilesService.Init(this);
            UserDialogs.Init(this);

            // Initialize and load app
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // Enable webview inspection in chrome
            WebView.SetWebContentsDebuggingEnabled(true);
        }
    }
}
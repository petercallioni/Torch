using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Views;

namespace App1
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Camera camera;
        bool light;
        Camera.Parameters parameters;
        Button flashOnButton;
        Button flashOffButton;
        protected override void OnCreate(Bundle bundle)
        {
            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);

            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            flashOnButton = FindViewById<Button>(Resource.Id.flashOnButton);
            flashOffButton = FindViewById<Button>(Resource.Id.flashOffButton);


            if (!hasFlash)
            {
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show(); ;
                return;
            }

            camera = Camera.Open();
            parameters = camera.GetParameters();
            changeButtons();

            flashOnButton.Click += delegate
            {
                parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                camera.SetParameters(parameters);
                camera.StartPreview();
                changeButtons();
            };

            flashOffButton.Click += delegate
            {
                parameters.FlashMode = Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.StartPreview();
                changeButtons();
            };
        }
        private void changeButtons()
        {
            if (!parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff))
            {
                light = true;
                flashOnButton.Visibility = ViewStates.Gone;
                flashOffButton.Visibility = ViewStates.Visible;
            }
            else
            {
                light = false;
                flashOnButton.Visibility = ViewStates.Visible;
                flashOffButton.Visibility = ViewStates.Gone;
            }
        }
    }
}


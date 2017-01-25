using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Views;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using static Android.Resource;
using Android.Content;
using Android.Runtime;
using System.Reflection;

namespace App1
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static readonly int ButtonClickNotificationId = 1000;
        private Camera camera;
        bool light;
        Camera.Parameters parameters;
        Button flashTorchOnButton;
        Button flashOffButton;
        CheckBox serviceCheckBox;
        protected override void OnCreate(Bundle bundle)
        {
            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);

            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            serviceCheckBox = FindViewById<CheckBox>(Resource.Id.turnOnFlashNotification);
            flashTorchOnButton = FindViewById<Button>(Resource.Id.flashTorchOnButton);
            flashOffButton = FindViewById<Button>(Resource.Id.flashOffButton);


            if (!hasFlash)
            {
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show();
                return;
            }

            camera = Camera.Open();
            parameters = camera.GetParameters();
            ChangeButtons();
            Intent TorchService = new Intent(this, typeof(TorchService));
            serviceCheckBox.Click += delegate
            {
                if (serviceCheckBox.Checked)
                {
                    
                    StartService(TorchService);
                }
                else
                {
                    StopService(TorchService);
                }
            };

            flashTorchOnButton.Click += delegate
            {
                parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                camera.SetParameters(parameters);
                camera.StartPreview();
                ChangeButtons();
            };

            flashOffButton.Click += delegate
            {
                parameters.FlashMode = Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.StartPreview();
                ChangeButtons();
            };

        }
        private void ChangeButtons()
        {
            if (!parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff))
            {
                light = true;
                flashTorchOnButton.Visibility = ViewStates.Gone;
                flashOffButton.Visibility = ViewStates.Visible;
            }
            else
            {
                light = false;
                flashTorchOnButton.Visibility = ViewStates.Visible;
                flashOffButton.Visibility = ViewStates.Gone;
            }
        }
    }
}


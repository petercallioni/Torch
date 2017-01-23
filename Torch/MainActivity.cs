using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Views;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using static Android.Resource;
using Android.Content;

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
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show(); ;
                return;
            }

            camera = Camera.Open();
            parameters = camera.GetParameters();
            ChangeButtons();

            serviceCheckBox.Click += delegate
            {
                if (serviceCheckBox.Checked)
                {

                }
                else
                {

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
        private void StartNotification()
        {
            // Build the notification:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
               // .SetContentIntent(resultPendingIntent)  // Start 2nd activity when the intent is clicked.
                .SetContentTitle("Button Clicked")      // Set its title
                .SetSmallIcon(Resource.Drawable.Icon)  // Display this icon
                .SetContentText("test"); // The message to display.

            // Finally, publish the notification:
            NotificationManager notificationManager =
                (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(ButtonClickNotificationId, builder.Build());
        }
        private void StartFlashNotificatioService()
        {

        }
    }
}


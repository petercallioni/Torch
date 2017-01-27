using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Content;
using Android.Views;

namespace App1
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class FlashLightActivity : Activity
    {
        //ui
        Button flashTorchOnButton;
        Button flashOffButton;
        CheckBox serviceCheckBox;

        private bool light;
        private Camera camera;

        protected override void OnCreate(Bundle bundle)
        {
            //if the phone as a flash to use
            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);

            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //ui instantiations
            serviceCheckBox = FindViewById<CheckBox>(Resource.Id.turnOnFlashNotification);
            flashTorchOnButton = FindViewById<Button>(Resource.Id.flashTorchOnButton);
            flashOffButton = FindViewById<Button>(Resource.Id.flashOffButton);

            if (!hasFlash)
            {
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show();
                return;
            }
            //turn on notification action button
            Intent TorchService = new Intent(this, typeof(FlashlightNotificationService));
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

            //other buttons
            flashTorchOnButton.Click += delegate
            {
                SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                ChangeButtons();
                light = true;
            };

            flashOffButton.Click += delegate
            {
                SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                ChangeButtons();
                light = false;
            };
        }
        protected override void OnNewIntent(Intent intent)
        {
            light = intent.GetBooleanExtra("flashStatus", false);
            ChangeButtons();
            base.OnNewIntent(intent);
        }
        protected override void OnStart()
        {
            QueryTorchStatus();
            base.OnStart();
        }

        private void QueryTorchStatus()
        {
            SendBroadcast(new Intent("com.callioni.Torch.Status"));
        }

        private void ChangeButtons()
        {
            if (light)
            {
                flashTorchOnButton.Visibility = ViewStates.Gone;
                flashOffButton.Visibility = ViewStates.Visible;
            }
            else
            {
                flashTorchOnButton.Visibility = ViewStates.Visible;
                flashOffButton.Visibility = ViewStates.Gone;
            }
        }
    }
}

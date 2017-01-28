using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using System.Linq;

namespace App1
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class FlashLightActivity : Activity
    {
        // ui
        private Button flashTorchOnButton;
        private Button flashOffButton;
        private CheckBox serviceCheckBox;

        private bool light;

        protected override void OnCreate(Bundle bundle)
        {
            // if the phone as a flash to use
            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);

            if (!hasFlash)
            {
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show();
                return;
            }
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // ui instantiations
            serviceCheckBox = FindViewById<CheckBox>(Resource.Id.turnOnFlashNotification);
            flashTorchOnButton = FindViewById<Button>(Resource.Id.flashTorchOnButton);
            flashOffButton = FindViewById<Button>(Resource.Id.flashOffButton);


            // turn on notification action button
            Intent TorchService = new Intent(this, typeof(FlashlightNotificationService));


            foreach (string service in GetRunningServices())
            {
                if (service.Contains("FlashlightNotificationService"))
                {
                    serviceCheckBox.Checked = true;
                }
            }
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

            // other buttons
            flashTorchOnButton.Click += delegate
            {
                SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                light = true;
                ChangeButtons();
            };

            flashOffButton.Click += delegate
            {
                SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                light = false;
                ChangeButtons();
            };
        }
        protected override void OnNewIntent(Intent intent)
        {
            int result = intent.GetIntExtra("flashStatus", 0);
            Toast.MakeText(this, "data recieved = " + result, ToastLength.Short).Show();
            switch (result)
            {
                case 0:
                    light = false;
                    break;
                case 1:
                    light = true;
                    break;
            }
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

        private IEnumerable<string> GetRunningServices()
        {
            var manager = (ActivityManager)GetSystemService(ActivityService);
            return manager.GetRunningServices(int.MaxValue).Select(
                service => service.Service.ClassName).ToList();
        }
    }
}

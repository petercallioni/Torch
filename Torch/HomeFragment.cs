using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TorchMain
{
    public class HomeFragment : Fragment
    {
        // ui
        private Button flashTorchOnButton;
        private Button flashOffButton;
        private CheckBox serviceCheckBox;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void ChangeButtons()
        {
            if (FlashLightActivity.light)
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
            var manager = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);
            return manager.GetRunningServices(int.MaxValue).Select(
                service => service.Service.ClassName).ToList();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.home_fragment, container, false);

            // ui instantiations
            serviceCheckBox = rootView.FindViewById<CheckBox>(Resource.Id.turnOnFlashNotification);
            flashTorchOnButton = rootView.FindViewById<Button>(Resource.Id.flashTorchOnButton);
            flashOffButton = rootView.FindViewById<Button>(Resource.Id.flashOffButton);

            // turn on notification action button
            Intent TorchService = new Intent(Application.Context, typeof(FlashlightNotificationService));

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

                    Application.Context.StartService(TorchService);
                }
                else
                {
                    Application.Context.StopService(TorchService);
                }
            };

            // other buttons
            flashTorchOnButton.Click += delegate
            {
                Application.Context.SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                FlashLightActivity.light = true;
                ChangeButtons();
            };

            flashOffButton.Click += delegate
            {
                Application.Context.SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                FlashLightActivity.light = false;
                ChangeButtons();
            };
            
            return rootView;
        }
    }
}
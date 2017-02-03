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
        private ImageButton flashTorchOnButton;
        private ImageButton flashOffButton;
        private CheckBox serviceCheckBox;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnResume()
        {
            ChangeButtons();
            base.OnResume();
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

       

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.home_fragment, container, false);

            // ui instantiations
            flashTorchOnButton = rootView.FindViewById<ImageButton>(Resource.Id.flashTorchOnButton);
            flashOffButton = rootView.FindViewById<ImageButton>(Resource.Id.flashOffButton);

            ChangeButtons();

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
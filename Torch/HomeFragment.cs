using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;


namespace TorchMain
{
    public class HomeFragment : Fragment
    {
        // ui
        private ImageButton flashOnButton;
        private ImageButton flashOffButton;

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
            if (FlashLightActivity.Light)
            {
                flashOnButton.Visibility = ViewStates.Gone;
                flashOffButton.Visibility = ViewStates.Visible;
            }
            else
            {
                flashOnButton.Visibility = ViewStates.Visible;
                flashOffButton.Visibility = ViewStates.Gone;
            }
        }

       

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.home_fragment, container, false);

            // ui instantiations
            flashOnButton = rootView.FindViewById<ImageButton>(Resource.Id.flashTorchOnButton);
            flashOffButton = rootView.FindViewById<ImageButton>(Resource.Id.flashOffButton);

            ChangeButtons();

            // other buttons
            flashOnButton.Click += delegate
            {
                Application.Context.SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
            };

            flashOffButton.Click += delegate
            {
                Application.Context.SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
            };
            
            return rootView;
        }
    }
}
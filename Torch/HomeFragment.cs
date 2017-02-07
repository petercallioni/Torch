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
        private ImageButton buttonDown;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnResume()
        {
            ChangeButtons();
            base.OnResume();
        }

        // toggles the big ol' button
        public void ChangeButtons()
        {
            if (FlashLightActivity.Light)
            {
                flashOnButton.Visibility = ViewStates.Gone;
                flashOffButton.Visibility = ViewStates.Visible;
                buttonDown.Visibility = ViewStates.Gone;
            }
            else
            {
                flashOnButton.Visibility = ViewStates.Visible;
                flashOffButton.Visibility = ViewStates.Gone;
                buttonDown.Visibility = ViewStates.Gone;
            }
        }



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.home_fragment, container, false);

            // ui instantiations
            flashOnButton = rootView.FindViewById<ImageButton>(Resource.Id.flashTorchOnButton);
            flashOffButton = rootView.FindViewById<ImageButton>(Resource.Id.flashOffButton);
            buttonDown = rootView.FindViewById<ImageButton>(Resource.Id.buttonDown);


            ChangeButtons();

            // buttons
            flashOnButton.Touch += TouchButton;
            flashOffButton.Touch += TouchButton;
            return rootView;

        }
        private void TouchButton(object sender, View.TouchEventArgs touchEventArgs)
        {
            ImageButton btn = (ImageButton)sender;
            int x = btn.Width;
            int y = btn.Height;

            switch (touchEventArgs.Event.Action)
            {
                case MotionEventActions.Down:
                    btn.Visibility = ViewStates.Gone;
                    buttonDown.Visibility = ViewStates.Visible;
                    break;
                case MotionEventActions.Up:
                     if(touchEventArgs.Event.GetX() > x || // moved out to the right
                        touchEventArgs.Event.GetY() > y || // moved out to the bottom
                        touchEventArgs.Event.GetX() < 0 || // moved out to the left
                        touchEventArgs.Event.GetY() < 0)   // moved out to the top
                     {
                         btn.Visibility = ViewStates.Visible;
                         buttonDown.Visibility = ViewStates.Gone;
                        return;
                     }
                    Application.Context.SendBroadcast(new Intent("com.callioni.Torch.Toggle"));
                    break;

            }
        }
    }
}
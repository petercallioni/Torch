using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace TorchMain
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/Icon")]
    public class FlashLightActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        Intent TorchService;
        ISharedPreferences sharedPreferences;
        ISharedPreferencesEditor editor;
        NavigationView navigationView;
        private const string FLASHLIGHT_SERVICE = "Flashlight_Service";
        private const string FLASHLIGHT_SERVICE_ICON = "Flashlight_Service_Icon";

        public static bool InForground { get; set; } = false;

        public static bool Light { get; set; }

        protected override void OnResume()
        {
            InForground = true;
            if (sharedPreferences.GetBoolean(FLASHLIGHT_SERVICE, false))
            {
                navigationView.Menu.FindItem(Resource.Id.toggleService).SetChecked(true);

                if (sharedPreferences.GetBoolean("notificationServiceRunning", false))
                {
                    StartService(new Intent(Application.Context, typeof(FlashlightNotificationService)));
                }
            }
            if (sharedPreferences.GetBoolean(FLASHLIGHT_SERVICE_ICON, false))
            {
                navigationView.Menu.FindItem(Resource.Id.toggleServiceIcon).SetChecked(true);
            }
            if (sharedPreferences.GetInt("NotificationStyle", 0) == 0)
            {
                navigationView.Menu.FindItem(Resource.Id.toggleNotificationStyle).SetChecked(true);
            }
            else if (sharedPreferences.GetInt("NotificationStyle", 0) == 1)
            {
                navigationView.Menu.FindItem(Resource.Id.onOffNotificationStyle).SetChecked(true);
            }
            base.OnResume();
        }

        protected override void OnPause()
        {
            InForground = false;
            base.OnPause();
        }

        protected override void OnCreate(Bundle bundle)
        {


            // if the phone as a flash to use
            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            editor = sharedPreferences.Edit();

            if (sharedPreferences.GetInt("NotificationStyle", 0) == 0)
            {
                editor.PutInt("NotificationStyle", 0);
            }
            // turn on notification action button
            TorchService = new Intent(Application.Context, typeof(FlashlightNotificationService));


            if (!hasFlash)
            {
                Toast.MakeText(this, "This camera does not have a flash", ToastLength.Short).Show();
                return;
            }
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);


            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            // Attach item selected handler to navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            int[][] states = new int[][] {

                new int[] {-Android.Resource.Attribute.StateEnabled}, // disabled
                new int[] {-Android.Resource.Attribute.StateChecked}, // unchecked
                new int[] { Android.Resource.Attribute.StatePressed },  // pressed
                new int[] { Android.Resource.Attribute.StateEnabled} // enabled
            };

            int[] colors = new int[] {
                Android.Graphics.Color.Blue,
                Android.Graphics.Color.Red,
                Android.Graphics.Color.White,
                Android.Graphics.Color.Green
            };


            navigationView.ItemTextColor = new Android.Content.Res.ColorStateList(states, colors);
            navigationView.SetBackgroundColor(Android.Graphics.Color.Argb(255, 55, 71, 79));

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            var ft = FragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Add(Resource.Id.HomeFrameLayout, new HomeFragment(), "HomeFragment");
            ft.Commit();

            navigationView.Menu.SetGroupCheckable(Resource.Id.notificationStyleGroup, true, true);
        }

        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.toggleService):
                    // clicked while checked
                    if (e.MenuItem.IsChecked)
                    {
                        Application.Context.StopService(TorchService);
                        e.MenuItem.SetChecked(false);
                        editor.PutBoolean(FLASHLIGHT_SERVICE, false);
                        editor.Commit();
                    }
                    // clicked while unchecked
                    else
                    {
                        editor.PutBoolean(FLASHLIGHT_SERVICE, true);
                        Application.Context.StartService(TorchService);
                        Toast.MakeText(this, "The flashlight service has started", ToastLength.Long).Show();
                        e.MenuItem.SetChecked(true);
                        editor.Commit();
                    }
                    break;
                case (Resource.Id.toggleServiceIcon):
                    // toggling the icon while he service is on makes no sense
                    if (sharedPreferences.GetBoolean(FLASHLIGHT_SERVICE, false))
                    {
                        if (e.MenuItem.IsChecked)
                        {
                            Application.Context.StopService(TorchService);
                            Application.Context.StartService(TorchService);
                            e.MenuItem.SetChecked(false);
                            editor.PutBoolean(FLASHLIGHT_SERVICE_ICON, false);
                            editor.Commit();
                        }
                        else
                        {
                            Application.Context.StopService(TorchService);
                            Application.Context.StartService(TorchService);
                            editor.PutBoolean(FLASHLIGHT_SERVICE_ICON, true);
                            e.MenuItem.SetChecked(true);
                            editor.Commit();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Notification Service is not running", ToastLength.Short).Show();
                    }
                    break;
                case (Resource.Id.toggleNotificationStyle):
                    e.MenuItem.SetChecked(true);
                    navigationView.Menu.FindItem(Resource.Id.onOffNotificationStyle).SetChecked(false);
                    editor.PutInt("NotificationStyle", 0);
                    editor.Commit();
                    Application.Context.StopService(TorchService);
                    Application.Context.StartService(TorchService);
                    break;
                case (Resource.Id.onOffNotificationStyle):
                    e.MenuItem.SetChecked(true);
                    navigationView.Menu.FindItem(Resource.Id.toggleNotificationStyle).SetChecked(false);
                    editor.PutInt("NotificationStyle", 1);
                    editor.Commit();
                    Application.Context.StopService(TorchService);
                    Application.Context.StartService(TorchService);
                    break;
                default:
                    break;
            }
            // Close drawer
            drawerLayout.CloseDrawers();
        }
        protected override void OnNewIntent(Intent intent)
        {
            // receives the flashlight's status then tells the fragment to swap the buttons
            Light = intent.GetBooleanExtra("flashStatus", false);
            HomeFragment fragment = (HomeFragment)FragmentManager.FindFragmentByTag("HomeFragment");
            fragment.ChangeButtons();
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
    }
}

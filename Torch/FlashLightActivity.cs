using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using System.Linq;

using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace TorchMain
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class FlashLightActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;

        public static bool light { get; set; }

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

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

           
            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            var ft = FragmentManager.BeginTransaction();
            ft.AddToBackStack(null);
            ft.Add(Resource.Id.HomeFrameLayout, new HomeFragment(), "HomeFragment");
            ft.Commit();

        }
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
    
            }
            // Close drawer
            drawerLayout.CloseDrawers();
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

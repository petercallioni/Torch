using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Hardware;

namespace App1
{
    [Service]
    public class FlashlightService : Service
    {
        FlashlightServiceBinder binder;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("DemoService", "DemoService started");
            Toast.MakeText(this, "The demo service has started", ToastLength.Long).Show();
            StartServiceInForeground();
            return StartCommandResult.NotSticky;
        }
        void StartServiceInForeground()
        {
            var ongoing = new Notification(Resource.Drawable.Icon, "Torch Service in foreground");
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
            ongoing.SetLatestEventInfo(this, "DemoService", "DemoService is running in the foreground", pendingIntent);

            StartForeground((int)NotificationFlags.ForegroundService, ongoing);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Log.Debug("DemoService", "DemoService stopped");
        }


        public override IBinder OnBind(Intent intent)
        {
            binder = new FlashlightServiceBinder(this);
            return binder;
        }
    }
    public class FlashlightServiceBinder : Binder
    {
        FlashlightService service;

        public FlashlightServiceBinder(FlashlightService service)
        {
            this.service = service;
        }

        public FlashlightService GetFlashlightService()
        {
            return service;
        }
    }
}
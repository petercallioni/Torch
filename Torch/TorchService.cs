using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Hardware;
using System;

namespace TorchMain
{
    [Service]
    public class FlashlightNotificationService : Service
    {
        FlashlightNotificationServiceBinder binder;
        NotificationManager notificationManager;
        Notification.Builder notificationBuilder;
        ISharedPreferences sharedPreferences;

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        //on service start
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("TorchService", "TorchService started");
            StartNotificationService(intent);
            return StartCommandResult.NotSticky;
        }

        void StartNotificationService(Intent intent)
        {
            sharedPreferences = Application.Context.GetSharedPreferences("com.callioni.Torch_preferences", FileCreationMode.Private);
            notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            if(sharedPreferences.GetInt("NotificationStyle", 0) == 0)
            {
                var toggleFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Toggle"), PendingIntentFlags.UpdateCurrent);
                notificationBuilder = new Notification.Builder(this)
                  .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                  .SetContentIntent(toggleFlashIntent)
                  .SetOngoing(true);
                var editor = sharedPreferences.Edit();
                editor.PutBoolean("notificationServiceRunning", true);
                editor.Commit();

                // if the user turned off the icon
                if (sharedPreferences.GetBoolean("Flashlight_Service_Icon", false))
                {
                    notificationBuilder.SetPriority(-2);
                }

                // to dynamically change the notification icon
                if (sharedPreferences.GetBoolean("IsFlashOn", false))
                {
                    notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOn);
                    notificationBuilder.SetContentText("Toggle Flashlight Off");
                }
                else
                {
                    notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOff);
                    notificationBuilder.SetContentText("Toggle Flashlight On");
                }
            }
            else if (sharedPreferences.GetInt("NotificationStyle", 0) == 1)
            {
                var toggleFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Toggle"), PendingIntentFlags.UpdateCurrent);
                var onFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.On"), PendingIntentFlags.UpdateCurrent);
                var offFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Off"), PendingIntentFlags.UpdateCurrent);
                notificationBuilder = new Notification.Builder(this)
                  .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                  .AddAction(Resource.Drawable.flashlightOn, "On", onFlashIntent)
                  .AddAction(Resource.Drawable.flashlightOff, "Off", offFlashIntent)
                  .SetContentIntent(toggleFlashIntent)
                  .SetOngoing(true);
                var editor = sharedPreferences.Edit();
                editor.PutBoolean("notificationServiceRunning", true);
                editor.Commit();

                // if the user turned off the icon
                if (sharedPreferences.GetBoolean("Flashlight_Service_Icon", false))
                {
                    notificationBuilder.SetPriority(-2);
                }

                // to dynamically change the notification icon
                if (sharedPreferences.GetBoolean("IsFlashOn", false))
                {
                    notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOn);
                }
                else
                {
                    notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOff);
                }
            }

            notificationManager.Notify(SERVICE_RUNNING_NOTIFICATION_ID, notificationBuilder.Build());

        }

        public override void OnDestroy()
        {
            // stops the notification
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            var editor = sharedPreferences.Edit();
            editor.PutBoolean("notificationServiceRunning", false);
            editor.Commit();

            base.OnDestroy();
            Log.Debug("TorchService", "TorchService stopped");
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new FlashlightNotificationServiceBinder(this);
            return binder;
        }
    }
    public class FlashlightNotificationServiceBinder : Binder
    {
        FlashlightNotificationService service;

        public FlashlightNotificationServiceBinder(FlashlightNotificationService service)
        {
            this.service = service;
        }

        public FlashlightNotificationService GetFlashlightService()
        {
            return service;
        }
    }
}
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
            notificationManager =
    GetSystemService(Context.NotificationService) as NotificationManager;

            // intent to toggle the flashlight on/off
            var toggleFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Toggle"), PendingIntentFlags.UpdateCurrent);
            notificationBuilder = new Notification.Builder(this)
              .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
              .SetContentText(Resources.GetString(Resource.String.notification_text))
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
            if (sharedPreferences.GetString("intentPrefs", "default").Equals("NotificationFlashIconOn"))
            {
                notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOn);
            }
            else
            {
                notificationBuilder.SetSmallIcon(Resource.Drawable.flashlightOff);
            }
            notificationManager.Notify(SERVICE_RUNNING_NOTIFICATION_ID, notificationBuilder.Build());

        }

        public override void OnDestroy()
        {
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            var editor = sharedPreferences.Edit();
            editor.PutBoolean("notificationServiceRunning", true);
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

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "com.callioni.Torch.Toggle", "com.callioni.Torch.Status", Intent.ActionBootCompleted })]
    public class ToggleFlashlightService : BroadcastReceiver
    {
        static Camera camera = null;
        int status = 0;
        string name = "flashStatus";
        Intent sendBackStatus;
        Camera.Parameters parameters;

        public override void OnReceive(Context context, Intent intent)
        {
            Intent service = new Intent(context, typeof(FlashlightNotificationService));
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("com.callioni.Torch_preferences", FileCreationMode.Private);
            switch (intent.Action)
            {
                case "com.callioni.Torch.Toggle":
                    //if the flaslight is off
                    if (camera == null)
                    {
                        camera = Camera.Open();
                        parameters = camera.GetParameters();
                        parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                        camera.SetParameters(parameters);
                        camera.StartPreview();
                        if (FlashLightActivity.InForground)
                        {
                            QueryFlashStatus(context); // camera != null == button toggles to show off
                        }
                        if (sharedPreferences.GetBoolean("notificationServiceRunning", false))
                        {
                            var editor = sharedPreferences.Edit();
                            editor.PutString("intentPrefs", "NotificationFlashIconOn");
                            editor.Commit();
                            context.StartService(service);
                        }
                        return;

                    }

                    //if the camera is open
                    if (camera != null)
                    {
                        parameters = camera.GetParameters();
                        //if the flashlight is on
                        if (!parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff))
                        {
                            parameters.FlashMode = Camera.Parameters.FlashModeOff;
                            camera.SetParameters(parameters);
                        }
                        camera.StopPreview();
                        camera.Release(); 
                        camera = null;
                        if (FlashLightActivity.InForground)
                        {
                            QueryFlashStatus(context); // camera = null == button toggles to show on
                        }
                        if (sharedPreferences.GetBoolean("notificationServiceRunning", false))
                        {
                            var editor = sharedPreferences.Edit();
                            editor.PutString("intentPrefs", "NotificationFlashIconOff");
                            editor.Commit();
                            context.StartService(service);
                        }
                        return;
                    }
                    break;
                case "com.callioni.Torch.Status":
                    QueryFlashStatus(context);
                    break;
                case Intent.ActionBootCompleted:
                    if (sharedPreferences.GetBoolean("Flashlight_Service", false))
                    {
                        service.AddFlags(ActivityFlags.NewTask);
                        context.ApplicationContext.StartService(service);
                        service = null;
                    }
                    break;
            }
        }
        private void QueryFlashStatus(Context context)
        {
            if (camera != null)
            {
                parameters = camera.GetParameters();
                status = parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff) ? 0 : 1;
            }
            else
                status = 0;
            sendBackStatus = new Intent(context, typeof(FlashLightActivity));
            sendBackStatus.AddFlags(ActivityFlags.SingleTop);
            sendBackStatus.AddFlags(ActivityFlags.NewTask);
            sendBackStatus.PutExtra(name, status);
            context.StartActivity(sendBackStatus);
        }
    }
}
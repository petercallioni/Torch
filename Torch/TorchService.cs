using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Hardware;

namespace TorchMain
{
    [Service]
    public class FlashlightNotificationService : Service
    {
        FlashlightNotificationServiceBinder binder;
        NotificationManager notificationManager;

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        //on service start
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("TorchService", "TorchService started");
            StartService();
            return StartCommandResult.NotSticky;
        }

        void StartService()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("com.callioni.Torch_preferences", FileCreationMode.Private);
            notificationManager =
    GetSystemService(Context.NotificationService) as NotificationManager;

            // intent to toggle the flashlight on/off
            var toggleFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Toggle"), PendingIntentFlags.UpdateCurrent);
            var notificationBuilder = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetContentIntent(toggleFlashIntent)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true);

            if (sharedPreferences.GetBoolean("Flashlight_Service_Icon", false))
            {
                notificationBuilder.SetPriority(-2);
            }
            var notification = notificationBuilder.Build();

            notificationManager.Notify(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        public override void OnDestroy()
        {
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);
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
                        if(FlashLightActivity.InForground)
                        {
                            QueryFlashStatus(context); // camera != null == button toggles to show off
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
                        return;
                    }
                    break;
                case "com.callioni.Torch.Status":
                    QueryFlashStatus(context);
                    break;
                case Intent.ActionBootCompleted:
                    ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("com.callioni.Torch_preferences", FileCreationMode.Private);
                    if (sharedPreferences.GetBoolean("Flashlight_Service", false))
                      {
                     Intent service = new Intent(context, typeof(FlashlightNotificationService));
                     service.AddFlags(ActivityFlags.NewTask);
                     context.ApplicationContext.StartService(service);
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
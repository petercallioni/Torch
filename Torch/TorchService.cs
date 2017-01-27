using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Hardware;

namespace App1
{
    [Service]
    public class FlashlightNotificationService : Service
    {
        FlashlightNotificationServiceBinder binder;

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        //on service start
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("TorchService", "TorchService started");
            Toast.MakeText(this, "The flashlight service has started", ToastLength.Long).Show();
            StartServiceInForeground();
            return StartCommandResult.NotSticky;
        }

        void StartServiceInForeground()
        {
            // intent to toggle the flashlight on/off
            var toggleFlashIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.Toggle"), PendingIntentFlags.UpdateCurrent);
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetContentIntent(toggleFlashIntent)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true)
                .Build();

            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        public override void OnDestroy()
        {
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
    [IntentFilter(new[] { "com.callioni.Torch.Toggle", "com.callioni.Torch.Status" })]
    public class ToggleFlashlightService : BroadcastReceiver
    {
        static Camera camera = null;
        bool status = false;
        FlashlightToggleServiceBinder binder;

        public override void OnReceive(Context context, Intent intent)
        {
            Camera.Parameters parameters;

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
                        return;
                    }
                    break;
                case "com.callioni.Torch.Status":
                    if (camera != null)
                    {
                        parameters = camera.GetParameters();
                        status = parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff) ? false : true;
                    }
                    else
                        status = false;
                    Intent sendBackStatus = new Intent(context, typeof(FlashLightActivity));
                    sendBackStatus.AddFlags(ActivityFlags.SingleTop);
                    sendBackStatus.AddFlags(ActivityFlags.NewTask);
                    string name = "flashStatus";
                    sendBackStatus.PutExtra(name, status);
                    context.StartActivity(sendBackStatus);
                    break;
            }
        }
    }

    public class FlashlightToggleServiceBinder : Binder
    {
        ToggleFlashlightService service;

        public FlashlightToggleServiceBinder(ToggleFlashlightService service)
        {
            this.service = service;
        }

        public ToggleFlashlightService GetFlashlightService()
        {
            return service;
        }
    }
}
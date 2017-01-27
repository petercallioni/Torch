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
using Android.Hardware.Camera2;

namespace App1
{
    [Service]
    public class FlashlightNotificationService : Service
    {
        FlashlightNotificationServiceBinder binder;

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("TorchService", "TorchService started");
            Toast.MakeText(this, "The flashlight service has started", ToastLength.Long).Show();
            //  StartService(new Intent(this, typeof(ToggleFlashlightToggleService)));
            StartServiceInForeground();
            return StartCommandResult.NotSticky;
        }
        void StartServiceInForeground()
        {
             var flashOnIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.flashOn"), PendingIntentFlags.UpdateCurrent);
            var flashOffIntent = PendingIntent.GetBroadcast(this, 0, new Intent("com.callioni.Torch.flashOff"), PendingIntentFlags.UpdateCurrent);
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true)
                .AddAction(new Notification.Action(Resource.Drawable.Icon, "On", flashOnIntent))
                .AddAction(new Notification.Action(Resource.Drawable.Icon, "Off", flashOffIntent))
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
    [IntentFilter(new[] { "com.callioni.Torch.flashOn", "com.callioni.Torch.flashOff" })]
    public class ToggleFlashlightToggleService : BroadcastReceiver
    {
        static Camera camera = null;
        FlashlightToggleServiceBinder binder;

        public override void OnReceive(Context context, Intent intent)
        {
            Camera.Parameters parameters;
            switch (intent.Action)
            {
                case "com.callioni.Torch.flashOn":
                    if(camera == null)
                    {
                        camera = Camera.Open();
                    }
                    parameters = camera.GetParameters();
                    parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.StartPreview();
                    break;
                case "com.callioni.Torch.flashOff":
                    if (camera != null)
                    {
                        parameters = camera.GetParameters();
                        parameters.FlashMode = Camera.Parameters.FlashModeOff;
                        camera.SetParameters(parameters);
                        camera.StopPreview();
                        camera.Release();
                        camera = null;
                    }
                    break;
                default:
                    throw new CameraAccessException(CameraAccessErrorType.Error);
            }
        }
    }
    public class FlashlightToggleServiceBinder : Binder
    {
        ToggleFlashlightToggleService service;

        public FlashlightToggleServiceBinder(ToggleFlashlightToggleService service)
        {
            this.service = service;
        }

        public ToggleFlashlightToggleService GetFlashlightService()
        {
            return service;
        }
    }
}
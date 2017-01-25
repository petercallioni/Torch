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
    [Service(IsolatedProcess = true)]
    public class TorchService : Service
    {
        private TorchBroadCastReceiver _broadcastReceiver;
        // This is any integer value unique to the application.
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        Camera camera;
        static readonly string TAG = typeof(TorchService).FullName;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // The bulk of the OnStartComand code as been omitted for clarity. 
            Log.Debug("TorchService", "TorchService Started");

            Intent torchIntent = new Intent("com.callioni.TorchService");
            PendingIntent torchPendingIntent = PendingIntent.GetActivity(this, 0, torchIntent, 0);

            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.ApplicationName))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetContentIntent(torchPendingIntent)
                .SetOngoing(true)
                .Build();
                
            // Enlist this instance of the service as a foreground service
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            return new FlashlightServiceBinder(this);
        }
        public override bool OnUnbind(Intent intent)
        {
            // This method is optional
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnCreate()
        {
            Toast.MakeText(Application.Context, "Starting service", ToastLength.Short).Show();

            _broadcastReceiver = new TorchBroadCastReceiver();

            _broadcastReceiver.TorchEventRecieved+= (sender, e) => FlashlightToggle();

            RegisterReceiver(_broadcastReceiver, new IntentFilter("com.callioni.TorchService"));
            

            Log.Debug(TAG, "OnCreate");
          //  camera = Camera.Open();
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Toast.MakeText(Application.Context, "Stoping service", ToastLength.Short).Show();
            Log.Debug(TAG, "OnDestroy");
            base.OnDestroy();
        }

        public void FlashlightToggle()
        {
            Camera.Parameters parameters;
            parameters = camera.GetParameters();
            if (!parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff))
            {
                parameters.FlashMode = Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.StartPreview();
            }
            else
            {
                parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                camera.SetParameters(parameters);
                camera.StartPreview();
            }
        }
    }

    public class FlashlightServiceBinder : Binder
    {
        public FlashlightServiceBinder(TorchService service)
        {
            this.Service = service;
        }
        public TorchService Service { get; private set; }
    }

    public class FlashlightServiceConnection : Java.Lang.Object, IServiceConnection
    {
        static readonly string TAG = typeof(FlashlightServiceConnection).FullName;

        public bool IsConnected { get; private set; }
        public FlashlightServiceBinder Binder { get; private set; }

        public FlashlightServiceConnection()
        {
            IsConnected = false;
            Binder = null;
        }

        public void OnServiceConnected(ComponentName name, IBinder binder)
        {
            Binder = binder as FlashlightServiceBinder;
            IsConnected = Binder != null;
            Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
            IsConnected = false;
            Binder = null;
        }
    }

    [IntentFilter(new String[] { "com.callioni.TorchService"})]
    public class TorchBroadCastReceiver : BroadcastReceiver
    {
        public event EventHandler TorchEventRecieved;

        public override void OnReceive(Context context, Intent intent)
        {
           switch (intent.Action)
            {
                case "com.callioni.TorchService":
                    OnTorchEventRecieved(null);
                    break;
            }
        }

        protected virtual void OnTorchEventRecieved(EventArgs e)
        {
            if (TorchEventRecieved != null)
            {
                TorchEventRecieved(this, e);
            }
        }
    }
}
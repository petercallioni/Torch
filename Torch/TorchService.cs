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

namespace App1
{
    [Service(IsolatedProcess = true)]
    public class TorchService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        } 
        public void FlashlightToggle()
        {

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

}
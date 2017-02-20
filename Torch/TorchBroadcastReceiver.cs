using Android.App;
using Android.Content;
using Android.Hardware;

namespace TorchMain
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] {
        "com.callioni.Torch.Toggle",
        "com.callioni.Torch.On",
        "com.callioni.Torch.Off",
        "com.callioni.Torch.Status",
        Intent.ActionBootCompleted })]
    public class ToggleFlashlightService : BroadcastReceiver
    {
        //Camera is deprecated but I found it is much easier to use than Camera2
        static Camera camera = null;
        bool status = false;
        string name = "flashStatus";
        Intent sendBackStatus;
        Camera.Parameters parameters;
        Intent service;
        ISharedPreferences sharedPreferences;
        Context GlobalContext;

        public override void OnReceive(Context context, Intent intent)
        {
            service = new Intent(context, typeof(FlashlightNotificationService));
            sharedPreferences = Application.Context.GetSharedPreferences("com.callioni.Torch_preferences", FileCreationMode.Private);
            GlobalContext = context;
            switch (intent.Action)
            {
                case "com.callioni.Torch.Toggle":
                    //if the flaslight is off
                    if (camera == null)
                    {
                        TurnTorchOn();
                    }

                    //if the camera is open
                    else if (camera != null)
                    {
                        TurnTorchOff();
                    }
                    break;
                case "com.callioni.Torch.Status":
                    QueryFlashStatus();
                    break;
                case "com.callioni.Torch.On":
                    if (camera == null)
                        TurnTorchOn();
                    break;
                case "com.callioni.Torch.Off":
                    if (camera != null)
                        TurnTorchOff();
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
        private void QueryFlashStatus()
        {
            if (camera != null)
            {
                parameters = camera.GetParameters();
                status = parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff) ? false : true;
            }
            else
                status = false;
            sendBackStatus = new Intent(GlobalContext, typeof(FlashLightActivity));
            sendBackStatus.AddFlags(ActivityFlags.SingleTop);
            sendBackStatus.AddFlags(ActivityFlags.NewTask);
            sendBackStatus.PutExtra(name, status);
            GlobalContext.StartActivity(sendBackStatus);
        }
        private void TurnTorchOn()
        {
            camera = Camera.Open();
            parameters = camera.GetParameters();
            parameters.FlashMode = Camera.Parameters.FlashModeTorch;
            camera.SetParameters(parameters);
            camera.StartPreview();
            if (FlashLightActivity.InForground)
            {
                QueryFlashStatus(); // camera != null == button toggles to show off
            }
            if (sharedPreferences.GetBoolean("notificationServiceRunning", false))
            {
                var editor = sharedPreferences.Edit();
                editor.PutBoolean("IsFlashOn", true);
                editor.Commit();
                GlobalContext.StartService(service);
            }
            return;
        }
        private void TurnTorchOff()
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
                QueryFlashStatus(); // camera = null == button toggles to show on
            }
            if (sharedPreferences.GetBoolean("notificationServiceRunning", false))
            {
                var editor = sharedPreferences.Edit();
                editor.PutBoolean("IsFlashOn", false);
                editor.Commit();
                GlobalContext.StartService(service);
            }
            return;
        }
    }
}
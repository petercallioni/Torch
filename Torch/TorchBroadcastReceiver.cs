using Android.App;
using Android.Content;
using Android.Hardware;

namespace TorchMain
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "com.callioni.Torch.Toggle", "com.callioni.Torch.Status", Intent.ActionBootCompleted })]
    public class ToggleFlashlightService : BroadcastReceiver
    {
        //Camera is deprecated but I found it is much easier to use than Camera2
        static Camera camera = null;
        bool status = false;
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
                            editor.PutBoolean("IsFlashOn", true);
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
                            editor.PutBoolean("IsFlashOn", false);
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
                status = parameters.FlashMode.Equals(Camera.Parameters.FlashModeOff) ? false : true;
            }
            else
                status = false;
            sendBackStatus = new Intent(context, typeof(FlashLightActivity));
            sendBackStatus.AddFlags(ActivityFlags.SingleTop);
            sendBackStatus.AddFlags(ActivityFlags.NewTask);
            sendBackStatus.PutExtra(name, status);
            context.StartActivity(sendBackStatus);
        }
    }
}
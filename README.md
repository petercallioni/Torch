# Torch

This application is a personal experiement of mine as it uses Xamarin to code for  the Android platform using C# instead of the native Java. This a simple application to use the phone's camera flash as a flashlight. This program has two modes:

Mode 1) The standard mode where the main application presents you with a on/off button for the flashlight. Simple. 

Mode2) In the main application's settings you can enable the notification service. This service provides a sticky notificiation that allows the user to toggle the flash on or off from the directly from the notification list. This allows you to turn the flash on or off without interrupting whatever you are doing.

There are four .cs files in this project FlashlightActivity.cs, HomeFragment.cs, and TorchService.cs and TorchBroadcastReceiver.cs. 

FlashlightActivity.cs contains the code to create the main menu (using a navigation drawer) and handle the menu clicks. It also changes the buttons if the user toggles the torch from the notification, while the main application is open.

HomeFragment.cs handles the button clicks from the main application. When the button is clicked it sends a broadcast to toggle the flashlight, which is where TorchBroadcastReceiver.cs comes in.

TorchService is responsible for creating and running the sticky notification to toggle the flashlight. Depending on the user settings, the notitification can be created with a priority of 0 (defualt), which shows the icon in the system bar  AND on the lock screen (which is time saving) or -2 (minimum), which removes the icon in the system bar but aalso removes the notification from the lock screen. Shared preferences are used to share these settings.

TorchBroadcastReceiver.cs is the main meat of the application and is responsible for most of the main logic with turn the flash on/off and toggling buttons and icons the change. Most of the class is a swith acting on the specific intent recieved. A Toggle broadcast makes the receiver toggle the flashlight. A Status intent is used in two situations: if the main application starts up and the flash is already on, it toggles the button fro on to off, and if the main application is in the foreground, it will toggle the button. The ActionBootCompleted starts the notification service on boot if the user had the service turned on. Both the notification toggle and the toggle from the main application use this class to toggle the light as it uses the same code and is consistent.

This accomplishes it's task perfectly, except for two main limitations. Due to the use of Broadcasts, although consistent, performance may be poor with high response times. This is not always the case, and depends specifically on the individual device running the application. The response time is not long enough to be aggravating, but enough to be noticeable. 

The second limitation is that the flashlight is inherently tied to the camera; as such, having the flashlight on will cause any applications the use the camera to stop working until the flashlight is turned off. There is apparently complex ways to accomplish this using Camera2 (instead of just camera, which is also deprecated, even though it is simpler and easier to use), however, this would have been complex to accomplish in C#.

The Icons where made by http://www.freepik.com from http://www.flaticon.com and is licensed with the Creative Commons 3.0 Licence, http://creativecommons.org/licenses/by/3.0/. The original icon was black and white, which I coloured for the Icon.

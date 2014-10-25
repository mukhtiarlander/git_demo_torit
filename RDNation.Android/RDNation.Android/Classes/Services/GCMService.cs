using System;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Util;
using RDNation.Droid.Classes.Account;
using RDNation.Droid.Classes.Configuration;
using RDNation.Droid.Classes.Receivers;
using RDN.Portable.Settings.Enums;
using RDN.Portable.Config.Enums;


//VERY VERY VERY IMPORTANT NOTE!!!!
// Your package name MUST NOT start with an uppercase letter.
// Android does not allow permissions to start with an upper case letter
// If it does you will get a very cryptic error in logcat and it will not be obvious why you are crying!
// So please, for the love of all that is kind on this earth, use a LOWERCASE first letter in your Package Name!!!!
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")] //, ProtectionLevel = Android.Content.PM.Protection.Signature)]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace RDNation.Droid.Classes.Services
{
    //You must subclass this!
    [BroadcastReceiver(Permission = GCMConstants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : PushHandlerBroadcastReceiverBase<PushHandlerService>
    {
        //IMPORTANT: Change this to your own Sender ID!
        //The SENDER_ID is your Google API Console App Project ID.
        //  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
        //  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
        //  where 785671162406 is the project id, which is the SENDER_ID to use!
        public static string[] SENDER_IDS = new string[] { "493035910025" };

        public const string TAG = "RDNation-GCM";
    }

    [Service] //Must use the service tag
    public class PushHandlerService : PushHandlerServiceBase
    {
        public PushHandlerService() : base(PushHandlerBroadcastReceiver.SENDER_IDS) { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose(PushHandlerBroadcastReceiver.TAG, "GCM Registered: " + registrationId);

            Task.Factory.StartNew(
                               () =>
                               {
                                   var user = Settings.GetUserPreferences((Context)this);
                                   if (!user.IsRegisteredForNotifications)
                                   {
                                       PushClient.CheckDevice(this);
                                       PushClient.CheckManifest(this);
                                       PushClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);

                                       if (!string.IsNullOrEmpty(registrationId))
                                       {
                                           MobileNotification.SendNotificationId(user.MemberId.ToString(), registrationId, this);
                                           user.IsRegisteredForNotifications = true;
                                           user.RegistrationIdForNotifications = registrationId;
                                           Settings.SaveUserPreferences(this, user);
                                       }

                                   }
                               });

            //createNotification("PushSharp-GCM Registered...", "The device has been Registered, Tap to View!");
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Verbose(PushHandlerBroadcastReceiver.TAG, "GCM Unregistered: " + registrationId);
            //Remove from the web service
            //	var wc = new WebClient();
            //	var result = wc.UploadString("http://your.server.com/api/unregister/", "POST",
            //		"{ 'registrationId' : '" + lastRegistrationId + "' }");

            //createNotification("PushSharp-GCM Unregistered...", "The device has been unregistered, Tap to View!");
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info(PushHandlerBroadcastReceiver.TAG, "GCM Message Received!");

            try
            {
                var msg = new StringBuilder();

                if (intent != null && intent.Extras != null)
                {
                    foreach (var key in intent.Extras.KeySet())
                        msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
                    string header = intent.Extras.Get("head").ToString();
                    string message = intent.Extras.Get("message").ToString();
                    string type = intent.Extras.Get("nottype").ToString();

                    createNotification(header, message, (MobileNotificationTypeEnum)Enum.Parse(typeof(MobileNotificationTypeEnum), type));
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, this);
            }

            ////Store the message
            //var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            //var edit = prefs.Edit();
            //edit.PutString("last_msg", msg.ToString());
            //edit.Commit();


        }

        protected override bool OnRecoverableError(Context context, string errorId)
        {
            Log.Warn(PushHandlerBroadcastReceiver.TAG, "Recoverable Error: " + errorId);

            return base.OnRecoverableError(context, errorId);
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Error(PushHandlerBroadcastReceiver.TAG, "GCM Error: " + errorId);
        }

        void createNotification(string title, string desc, MobileNotificationTypeEnum messageType)
        {
            //Create notification
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            Intent uiIntent;
            if (messageType == MobileNotificationTypeEnum.Game)
            {
                uiIntent = new Intent(this, typeof(GamesActivity));
            }
            else
                uiIntent = new Intent(this, typeof(Main));
            //Create the notification
            var notification = new Notification(Resource.Drawable.icon, title);

            //Auto cancel will remove the notification once the user touches it
            notification.Flags = NotificationFlags.AutoCancel;

            //Set the notification info
            //we use the pending intent, passing our ui intent over which will get called
            //when the notification is tapped.
            notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

            //Show the notification
            notificationManager.Notify(1, notification);
        }
    }
}


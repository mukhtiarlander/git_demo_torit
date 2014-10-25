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

namespace RDNation.Droid.Classes.Helpers
{
    public static class AdMobHelper
    {
        //this is where we had specified: admob6sample.admob;
        private static IntPtr _helperClass = JNIEnv.FindClass("rdnation/android/admob/AdMobHelper");


        /// <summary>
        /// Refreshed the ad for the view
        /// </summary>
        /// <param name="view"></param>
        public static void LoadAd(View view)
        {
            IntPtr methodId = JNIEnv.GetStaticMethodID(_helperClass, "loadAd", "(Landroid/view/View;)V");
            JNIEnv.CallStaticVoidMethod(_helperClass, methodId, new JValue(view));
        }

        /// <summary>
        /// Destroys the ad
        /// </summary>
        /// <param name="view"></param>
        public static void Destroy(View view)
        {
            IntPtr methodId = JNIEnv.GetStaticMethodID(_helperClass, "destroy", "(Landroid/view/View;)V");
            JNIEnv.CallStaticVoidMethod(_helperClass, methodId, new JValue(view));
        }
    }
}
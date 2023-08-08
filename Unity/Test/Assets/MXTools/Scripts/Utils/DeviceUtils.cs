using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
namespace DeviceUtils
{
    /// <summary>
    /// 
    /// </summary>
    public class Device
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unity_player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaClass context = new AndroidJavaClass("android.content.Context");
        public static AndroidJavaObject activity = unity_player.GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
#endif
        public static int SDK_INT
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                int sdk = Device.version.GetStatic<int>("SDK_INT");
                return sdk;
#else
                return 0;
#endif
            }
        }
        public static string RELEASE
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string release = Device.version.GetStatic<string>("RELEASE");
                return "Android " + release;
#else
                return "Unknow";
#endif
            }
        }

    }

    

    /// <summary>
    /// 
    /// </summary>
    public class Vibration
    {


        public static void trigger()
        {
            Vibration.vibrate(20, true);
        }

        protected static void vibrate(long milliseconds, bool light)
        {
            int amplitude = -1;
            if(light) { amplitude = 40; }

            Vibration.vibrate(milliseconds, amplitude, false, true, false);
        }


        /// <summary>
        /// 在Vibrate（）方法中，我们检查Android的版本，如果版本大于或等于Android 8.0（Build.VERSION_CODES.O），
        /// 我们使用VibrationEffect.CreateOneShot（）方法创建一个持续时间为20毫秒、默认振幅的震动效果。
        /// </summary> 
        public static void vibrate(long milliseconds, int amplitude = -1, bool alarm = true, bool legacy = false, bool obsolete = false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string service_name = Device.context.GetStatic<string>("VIBRATOR_SERVICE");
            AndroidJavaObject vibrator = Device.activity.Call<AndroidJavaObject>("getSystemService", service_name);
            bool result = vibrator.Call<bool>("hasVibrator");
            if (!result)
            {
                return;
            }

            int sdk_version = Device.SDK_INT;
            if (sdk_version >= 31)
            {
                service_name = Device.context.GetStatic<string>("VIBRATOR_MANAGER_SERVICE");
                AndroidJavaObject manager = Device.activity.Call<AndroidJavaObject>("getSystemService", service_name);
                vibrator = manager.Call<AndroidJavaObject>("getDefaultVibrator");
            }

            bool support = vibrator.Call<bool>("hasAmplitudeControl");
            if (sdk_version >= 26 && support && !obsolete)
            {
                AndroidJavaClass VibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                if (VibrationEffect != null)
                {
                    if (amplitude < 0)
                    {
                        amplitude = VibrationEffect.GetStatic<int>("DEFAULT_AMPLITUDE");
                    }

                    AndroidJavaObject vibration_effect = VibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                    if (sdk_version >= 33 && !legacy)
                    {
                        AndroidJavaClass VibrationAttributes = new AndroidJavaClass("android.os.VibrationAttributes");

                        int usage_alarm = VibrationAttributes.GetStatic<int>("USAGE_ALARM");
                        int usage_ringtone = VibrationAttributes.GetStatic<int>("USAGE_RINGTONE");
                        int usage = usage_ringtone;
                        if(alarm)
                        {
                            usage = usage_alarm;
                        }
                        AndroidJavaObject vibration_attributes = VibrationAttributes.CallStatic<AndroidJavaObject>("createForUsage", usage_alarm);

                        vibrator.Call("vibrate", vibration_effect, vibration_attributes);
                    }
                    else
                    {
                        vibrator.Call("vibrate", vibration_effect);
                    }
                }
            }
            else
            {
                vibrator.Call("vibrate", milliseconds);
            }

#endif
        }
    }

}

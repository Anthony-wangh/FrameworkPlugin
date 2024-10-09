using UnityEngine;

namespace Framework.Common
{
    public class Platform
    {
        public static bool IsEditor => Application.isEditor;
        public static bool IsAndroid => Application.platform == RuntimePlatform.Android && !IsEditor;
        public static bool IsIPhone => Application.platform == RuntimePlatform.IPhonePlayer && !IsEditor;
        public static bool IsWindows => Application.platform == RuntimePlatform.WindowsPlayer && !IsEditor;
        public static bool IsMac => Application.platform == RuntimePlatform.OSXPlayer && !IsEditor;
    }
}

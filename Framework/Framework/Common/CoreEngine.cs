using System;
using UnityEngine;

// ReSharper disable ArrangeTypeMemberModifiers

namespace Framework.Common
{
    /// <summary>
    /// 整个app的启动引擎，所以update都走这边
    /// </summary>
    public class CoreEngine : MonoBehaviour
    {
        public static CoreEngine Inst { get; }
        public static Action OnUpdate;
        public static Action OnLateUpdate;
        public static Action OnFixedUpdate;
        public static Action OnGUIEvent;
        private static float _lastUpdateTime;
        public static bool Busy => Time.realtimeSinceStartup - _lastUpdateTime > 0.018f;

        static CoreEngine()
        {
            GameObject obj = new GameObject(typeof(CoreEngine).ToString()) { hideFlags = HideFlags.HideInHierarchy };
            DontDestroyOnLoad(obj);
            Inst = obj.AddComponent<CoreEngine>();
        }

        void Update()
        {
            _lastUpdateTime = Time.realtimeSinceStartup;
            OnUpdate?.Invoke();
        }

        void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
        void OnGUI()
        {
            OnGUIEvent?.Invoke();
        }
    }
}

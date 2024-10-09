using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility
{
    /// <summary>
    /// 对象删除与回收(内存不足 自动触发)
    /// </summary>
    public class ObjectUtility : UtilityBase
    {
        private readonly List<Object> _objList = new List<Object>();
        private bool _isDirty;

        private const int UnloadTime = 3;

        private float _curTime;

        protected override void OnInit()
        {
            CoreEngine.OnUpdate += OnUpdate;
            Application.lowMemory += LowMemory;
        }


        private void LowMemory()
        {
            UnloadUnusedAssets();
        }

        private void OnUpdate()
        {
            if (_objList == null || _objList.Count == 0)
            {
                Unload();
                return;
            }
            //这一帧 有的地方执行 卡顿了，下一帧在删除
            if (CoreEngine.Busy)
                return;

            _curTime = 0;
            var obj = _objList[0];
            _objList.RemoveAt(0);
            if (obj == null)
                return;

            Object.Destroy(obj);
        }

        private void Unload()
        {
            if (!_isDirty)
                return;

            _curTime += Time.deltaTime;
            if (_curTime >= UnloadTime)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                _isDirty = false;
            }
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj"></param>
        public void Destroy(Object obj)
        {
            if (obj == null)
                return;

            _objList.Add(obj);
        }

        /// <summary>
        /// 立刻删除对象
        /// </summary>
        /// <param name="obj"></param>
        public void DestroyImmediate(GameObject obj)
        {
            if (obj == null)
                return;

            Object.DestroyImmediate(obj);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj"></param>
        public void Destroy(GameObject obj)
        {
            if (obj == null)
                return;

            obj.SetActive(false);
            _objList.Add(obj);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj"></param>
        public void Destroy(Transform obj)
        {
            if (obj == null)
                return;

            Destroy(obj.gameObject);
        }

        /// <summary>
        /// 回收未使用的资源（会触发GC回收）
        /// </summary>
        public void UnloadUnusedAssets()
        {
            _isDirty = true;
        }
    }
}

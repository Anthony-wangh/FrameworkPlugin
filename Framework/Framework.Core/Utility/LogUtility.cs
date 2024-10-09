using UnityEngine;

namespace Framework.Core.Utility
{

    /// <summary>
    /// 日志输出工具
    /// </summary>
    public class LogUtility : UtilityBase
    {
        /// <summary>
        /// 启动日志输出 (Error 一直会打印)
        /// </summary>
        private bool _enable = true;

        public void Disable()
        {
            _enable = false;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="msg"></param>
        public void Log(object msg)
        {
            if (_enable == false)
                return;

            Debug.Log(msg);
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="msg"></param>
        public void LogWarning(object msg)
        {
            if (_enable == false)
                return;

            Debug.LogWarning(msg);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="msg"></param>
        public void LogError(object msg)
        {
            Debug.LogError(msg);
        }
    }
}

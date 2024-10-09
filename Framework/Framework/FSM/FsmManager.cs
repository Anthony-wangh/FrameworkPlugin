using UnityEngine;

namespace Framework.Fsm
{
    /// <summary>
    /// 状态机管理类
    /// </summary>
    public class FsmManager
    {
        /// <summary>
        /// 实例化状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T InstantiateFsm<T>() where T : IFsmManager,new()
        {
            T t = new T();
            FsmManageBase fsmManageBase = t as FsmManageBase;
            fsmManageBase?.Init();
            return t;
        }
    }
}

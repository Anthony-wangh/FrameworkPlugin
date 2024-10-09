using UnityEngine;

namespace Framework.Fsm
{
    /// <summary>
    /// ״̬��������
    /// </summary>
    public class FsmManager
    {
        /// <summary>
        /// ʵ����״̬��
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

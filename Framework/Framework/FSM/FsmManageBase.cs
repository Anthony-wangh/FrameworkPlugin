using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Core;
using Framework.Core.Utility;

namespace Framework.Fsm
{
    /// <summary>
    /// ״̬���������
    /// </summary>
    public class FsmManageBase : AbstractBase, IFsmManager
    {
        public Dictionary<Type, IFsmState> FsmStateList { get; set; }
        public IFsmState CurState => _curState;
        private IFsmState _curState;

        protected LogUtility Log;
        /// <summary>
        /// ��ʼ��
        /// </summary>
        public  void Init()
        {
            Log = GetUtility<LogUtility>();
            CoreEngine.OnUpdate += OnUpdate;
            FsmStateList =new Dictionary<Type, IFsmState>();
            OnInit();
        }

        protected virtual void OnInit()
        {

        }
        /// <summary>
        /// ע��״̬
        /// </summary>
        /// <param name="fsm"></param>
        public void Register(IFsmState fsm)
        {
            var type = fsm.GetType();
            if (FsmStateList.ContainsKey(type))
            {
                Log.Log("�ظ����״̬��" + type.FullName);
                return;
            }
            FsmStateList.Add(type, fsm);
        }
        /// <summary>
        /// �л�״̬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>(params object[] param) where T : IFsmState
        {
            var type = typeof(T);
            if (!FsmStateList.ContainsKey(type))
            {
                Log.Log("��״̬��δע�᣺" + type);
                return;
            }
            if (_curState != null)
            {
                _curState.Exit();
            }
            var state = FsmStateList[type];
            _curState = state;
            Log.Log("ChangeState: " + type);
            state.Enter(param);
        }

        public void ChangeState(IFsmState state, params object[] param)
        {
            if (_curState != null)
            {
                _curState.Exit();
            }
            _curState = state;
            Log.Log("ChangeState: " + state.GetType());
            state.Enter(param);
        }
        private void OnUpdate()
        {
            if (_curState != null)
                _curState.Update();
        }
        /// <summary>
        /// ��ȡ״̬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IFsmState GetState<T>() where T : IFsmState
        {
            var type = typeof(T);
            if (FsmStateList.ContainsKey(type))
                return FsmStateList[type];
            return null;
        }

        /// <summary>
        /// ж��
        /// </summary>
        public virtual void Dispose()
        {
            if (_curState != null)
                _curState.Exit();
            _curState = null;
            CoreEngine.OnUpdate -= OnUpdate;
            FsmStateList.Clear();
        }
  
    
    }
}

using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Core;
using Framework.Core.Utility;

namespace Framework.Fsm
{
    /// <summary>
    /// ×´Ì¬»ú¹ÜÀí»ùÀà
    /// </summary>
    public class FsmManageBase : AbstractBase, IFsmManager
    {
        public Dictionary<Type, IFsmState> FsmStateList { get; set; }
        public IFsmState CurState => _curState;
        private IFsmState _curState;

        protected LogUtility Log;
        /// <summary>
        /// ³õÊ¼»¯
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
        /// ×¢²á×´Ì¬
        /// </summary>
        /// <param name="fsm"></param>
        public void Register(IFsmState fsm)
        {
            var type = fsm.GetType();
            if (FsmStateList.ContainsKey(type))
            {
                Log.Log("ÖØ¸´Ìí¼Ó×´Ì¬£º" + type.FullName);
                return;
            }
            FsmStateList.Add(type, fsm);
        }
        /// <summary>
        /// ÇÐ»»×´Ì¬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>(params object[] param) where T : IFsmState
        {
            var type = typeof(T);
            if (!FsmStateList.ContainsKey(type))
            {
                Log.Log("¸Ã×´Ì¬»¹Î´×¢²á£º" + type);
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
        /// »ñÈ¡×´Ì¬
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
        /// Ð¶ÔØ
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

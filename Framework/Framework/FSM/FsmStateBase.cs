using Framework.Core;

namespace Framework.Fsm
{
    public class FsmStateBase : AbstractBase, IFsmState
    {
        protected IFsmManager Fsm;
        public FsmStateBase(IFsmManager fsm)
        {
            Fsm = fsm;
        }
    
        /// <summary>
        /// ÇÐ»»×´Ì¬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>(params object[] param) where T : IFsmState
        {
            Fsm.ChangeState<T>(param);
        }

        /// <summary>
        /// ÇÐ»»×´Ì¬Ê¾Àý
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        public void ChangeState(IFsmState state, params object[] param)
        {
            Fsm.ChangeState(state,param);
        }

        /// <summary>
        /// ½øÈë×´Ì¬
        /// </summary>
        public virtual void Enter(params object[] param)
        {
            OnEnter(param);
        }
        /// <summary>
        /// ¸üÐÂ×´Ì¬
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }
        /// <summary>
        /// ÍË³ö×´Ì¬
        /// </summary>
        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnEnter(params object[] param)
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }
    
    }
}

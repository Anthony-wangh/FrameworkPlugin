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
        /// �л�״̬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ChangeState<T>(params object[] param) where T : IFsmState
        {
            Fsm.ChangeState<T>(param);
        }

        /// <summary>
        /// �л�״̬ʾ��
        /// </summary>
        /// <param name="state"></param>
        /// <param name="param"></param>
        public void ChangeState(IFsmState state, params object[] param)
        {
            Fsm.ChangeState(state,param);
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        public virtual void Enter(params object[] param)
        {
            OnEnter(param);
        }
        /// <summary>
        /// ����״̬
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }
        /// <summary>
        /// �˳�״̬
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

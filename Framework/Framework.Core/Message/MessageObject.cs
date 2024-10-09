using System;

namespace Framework.Core.Message
{
    public class MessageObject<T> : IMessageBase where T : IMessageBase
    {
        private Action<T> _action;
        public void Publish(T t)
        {
            _action?.Invoke(t);
        }

        public IUnsubscribeOnDestroy Subscribe(Action<T> func)
        {
            _action += func;
            return new MessageUnsubscribe(() => Unsubscribe(func));
        }

        public void Unsubscribe(Action<T> func)
        {
            if (_action == null)
                return;

            // ReSharper disable once DelegateSubtraction
            _action -= func;
        }

        public bool IsCanRemove()
        {
            if (_action == null)
                return true;

            if (_action.GetInvocationList().Length == 0)
                return true;

            return false;
        }
    }
}

using System;

namespace Framework.Core.Message
{
    public struct MessageUnsubscribe : IUnsubscribeOnDestroy
    {
        private Action _unsubscribe;
        public MessageUnsubscribe(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void UnsubscribeOnDestroy()
        {
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }
}

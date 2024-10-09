using System;
using System.Collections.Generic;

namespace Framework.Core.Message
{
    /// <summary>
    /// 事件系统
    /// </summary>
    public class MessageManager : ManagerBase, IMessage
    {
        private readonly Dictionary<Type, IMessageBase> _messages = new Dictionary<Type, IMessageBase>();

        public IUnsubscribeOnDestroy Subscribe<T>(Action<T> func) where T : IMessageBase
        {
            var type = typeof(T);
            if (!_messages.ContainsKey(type))
            {
                _messages.Add(type, new MessageObject<T>());
            }

            var msg = _messages[type] as MessageObject<T>;
            return msg?.Subscribe(func);
        }

        public void Unsubscribe<T>(Action<T> func) where T : IMessageBase
        {
            var type = typeof(T);
            if (!_messages.ContainsKey(type))
            {
                return;
            }

            var msg = _messages[type] as MessageObject<T>;
            if (msg == null)
                return;

            msg.Unsubscribe(func);
            if (msg.IsCanRemove())
            {
                _messages.Remove(type);
            }
        }

        public void Publish<T>(T t) where T : IMessageBase
        {
            var type = typeof(T);
            if (!_messages.ContainsKey(type))
            {
                return;
            }
            var msg = _messages[type] as MessageObject<T>;
            msg?.Publish(t);
        }

        public void Publish<T>() where T : IMessageBase, new()
        {
            var type = typeof(T);
            if (!_messages.ContainsKey(type))
            {
                return;
            }
            var msg = _messages[type] as MessageObject<T>;
            msg?.Publish(new T());
        }
    }
}

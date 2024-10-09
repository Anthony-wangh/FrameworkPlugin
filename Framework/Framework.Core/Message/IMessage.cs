using System;

namespace Framework.Core.Message
{
    public interface IMessage : IPublish, ISubscribe, IUnsubscribe
    {

    }

    public interface IPublish
    {
        void Publish<T>(T t) where T : IMessageBase;
        void Publish<T>() where T : IMessageBase, new();
    }

    public interface ISubscribe
    {
        IUnsubscribeOnDestroy Subscribe<T>(Action<T> func) where T : IMessageBase;
    }

    public interface IUnsubscribe
    {
        void Unsubscribe<T>(Action<T> func) where T : IMessageBase;
    }

    public interface IUnsubscribeOnDestroy
    {
        void UnsubscribeOnDestroy();
    }

    public interface IMessageBase
    {

    }
}

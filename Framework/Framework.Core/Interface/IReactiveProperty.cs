using System;
using Framework.Core.Message;

namespace Framework.Core.Interface
{
    public interface IReactiveProperty<T>
    {
        T Value { get; set; }
        void SetValueWithoutPublish(T newValue);
        void SetValueWithPublish(T newValue);
        IUnsubscribeOnDestroy SubscribeWithPublish(Action<T> onValueChanged);
        void Unsubscribe(Action<T> onValueChanged);
        IUnsubscribeOnDestroy Subscribe(Action<T> onValueChanged);
    }
}

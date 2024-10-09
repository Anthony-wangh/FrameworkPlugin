using System;
using Framework.Core.Interface;
using Framework.Core.Message;

namespace Framework.Core
{
    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        public ReactiveProperty(T defaultValue = default)
        {
            _value = defaultValue;
        }
        private Action<T> _onValueChanged;
        protected T _value;
        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && GetValue() == null) return;
                if (value != null && value.Equals(GetValue())) return;

                SetValue(value);
                _onValueChanged?.Invoke(value);
            }
        }

        protected virtual void SetValue(T newValue)
        {
            _value = newValue;
        }

        protected virtual T GetValue()
        {
            return _value;
        }

        public void SetValueWithoutPublish(T newValue)
        {
            SetValue(newValue);
        }

        public void SetValueWithPublish(T newValue)
        {
            SetValue(newValue);
            _onValueChanged?.Invoke(newValue);
        }

        public IUnsubscribeOnDestroy SubscribeWithPublish(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            onValueChanged?.Invoke(GetValue());
            return new ReactivePropertyUnsubscribe(() => { Unsubscribe(onValueChanged); });
        }

        public void Unsubscribe(Action<T> onValueChanged)
        {
            _onValueChanged -= onValueChanged;
        }

        public IUnsubscribeOnDestroy Subscribe(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            return new ReactivePropertyUnsubscribe(() => { Unsubscribe(onValueChanged); });
        }
    }

    public struct ReactivePropertyUnsubscribe : IUnsubscribeOnDestroy
    {
        private Action _unsubscribe;
        public ReactivePropertyUnsubscribe(Action unsubscribe)
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

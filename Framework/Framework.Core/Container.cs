using System;
using System.Collections.Generic;
using Framework.Core.Interface;

namespace Framework.Core
{
    internal class Container
    {
        private readonly Dictionary<Type, IInit> _instanceDict = new Dictionary<Type, IInit>();

        public void Bind<T>(T instance) where T : IInit
        {
            var key = typeof(T);

            if (_instanceDict.ContainsKey(key))
            {
                _instanceDict[key] = instance;
            }
            else
            {
                _instanceDict.Add(key, instance);
            }
        }

        public T Acquire<T>() where T : class, IInit
        {
            var key = typeof(T);

            if (_instanceDict.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public T GetOrNew<T>() where T : class, IInit, new()
        {
            var key = typeof(T);

            if (_instanceDict.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            var t = new T();
            t.Init();
            _instanceDict.Add(key, t);
            return t;
        }

        public void Unbind<T>() where T : class
        {
            var key = typeof(T);

            if (_instanceDict.ContainsKey(key))
            {
                _instanceDict.Remove(key);
            }
        }
    }
}

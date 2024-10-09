using Framework.Core.Interface;
using Framework.Core.Utility;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// control之间通过事件通信 不能直接调用
    /// </summary>
    public class Facade : IFacade
    {
        public static Facade Inst => _inst ?? (_inst = new Facade());

        private static Facade _inst;
        private readonly Dictionary<string, IControl> _controlDict = new Dictionary<string, IControl>();
        private LogUtility _log;
        private readonly Container _iocContainer = new Container();
        public void Init()
        {
            _log = GetUtility<LogUtility>();
        }

        public void Bind<T>(T instance) where T : class, IInit
        {
            instance.Init();
            _iocContainer.Bind(instance);
        }

        public T Acquire<T>() where T : class, IInit
        {
            return _iocContainer.Acquire<T>();
        }

        public T GetService<T>() where T : class, IService, new()
        {
            return _iocContainer.GetOrNew<T>();
        }

        public T GetManager<T>() where T : class, IManager, new()
        {
            return _iocContainer.GetOrNew<T>();
        }

        public T GetModel<T>() where T : class, IModel, new()
        {
            return _iocContainer.GetOrNew<T>();
        }

        public T GetUtility<T>() where T : class, IUtility, new()
        {
            return _iocContainer.GetOrNew<T>();
        }

        public void UnRegister<T>() where T : class, IControl
        {
            string key = typeof(T).ToString();
            if (_controlDict.TryGetValue(key, out var control))
            {
                control.Dispose();
                _controlDict.Remove(key);
            }
        }

        public void Register<T>() where T : class, IControl, new()
        {
            string key = typeof(T).ToString();
            if (_controlDict.ContainsKey(key))
            {
                _log.LogError("重复注册Control:" + key);
                return;
            }

            var t = new T();
            t.Init();
            _controlDict[key] = t;
        }

    }
}

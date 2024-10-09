using System;
using System.Collections.Generic;
using Framework.Core.Interface;
using Framework.Core.Message;

namespace Framework.Core
{
    public abstract class AbstractBase
    {
        private readonly List<IUnsubscribeOnDestroy> _unsubscribeList = new List<IUnsubscribeOnDestroy>();
        #region 消息
        protected void Publish<T>(T t) where T : IMessageBase
        {
            GetManager<MessageManager>().Publish(t);
        }

        protected void Publish<T>() where T : IMessageBase, new()
        {
            GetManager<MessageManager>().Publish<T>();
        }

        protected IUnsubscribeOnDestroy Subscribe<T>(Action<T> func) where T : IMessageBase
        {
            var unsubscribe = GetManager<MessageManager>().Subscribe(func);
            _unsubscribeList.Add(unsubscribe);
            return unsubscribe;
        }


        protected void Unsubscribe<T>(Action<T> func) where T : IMessageBase
        {
            GetManager<MessageManager>().Unsubscribe(func);
        }

        protected void UnsubscribeAll()
        {
            foreach (var unsubscribeOnDestroy in _unsubscribeList)
            {
                unsubscribeOnDestroy.UnsubscribeOnDestroy();
            }
            _unsubscribeList.Clear();
        }

        #endregion

        #region 获取

        protected T GetModel<T>() where T : class, IModel, new()
        {
            return Facade.Inst.GetModel<T>();
        }

        protected T GetManager<T>() where T : class, IManager, new()
        {
            return Facade.Inst.GetManager<T>();
        }

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return Facade.Inst.GetUtility<T>();
        }

        protected T Acquire<T>() where T : class, IInit
        {
            return Facade.Inst.Acquire<T>();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Framework.Core.Interface;
using Framework.Core.Message;
using UnityEngine;

namespace Framework.Core
{
    public class BehaviourBase : MonoBehaviour
    {
        private Facade _facade;
        private MessageManager _message;
        private readonly List<IUnsubscribeOnDestroy> _unsubscribeList = new List<IUnsubscribeOnDestroy>();
        void Awake()
        {
            _facade = Facade.Inst;
            _message = GetManager<MessageManager>();
            OnInit();
        }

        protected virtual void OnInit()
        {

        }

        void Start()
        {
            OnSubscribe();
        }

        void OnDestroy()
        {
            UnsubscribeAll();
            OnDispose();
        }

        protected virtual void OnSubscribe()
        {

        }

        protected virtual void OnDispose()
        {

        }

        public void UnsubscribeAll()
        {
            foreach (var unsubscribeOnDestroy in _unsubscribeList)
            {
                unsubscribeOnDestroy.UnsubscribeOnDestroy();
            }
            _unsubscribeList.Clear();
        }

        #region 消息
        protected void Publish<T>(T t) where T : IMessageBase
        {
            _message.Publish(t);
        }

        protected void Publish<T>() where T : IMessageBase, new()
        {
            _message.Publish<T>();
        }

        protected IUnsubscribeOnDestroy Subscribe<T>(Action<T> func) where T : IMessageBase
        {
            var unsubscribe = _message.Subscribe(func);
            _unsubscribeList.Add(unsubscribe);
            return unsubscribe;
        }


        protected void Unsubscribe<T>(Action<T> func) where T : IMessageBase
        {
            _message.Unsubscribe(func);
        }

        #endregion

        #region 获取

        protected T GetModel<T>() where T : class, IModel, new()
        {
            return _facade.GetModel<T>();
        }

        protected T GetManager<T>() where T : class, IManager, new()
        {
            return _facade.GetManager<T>();
        }

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return _facade.GetUtility<T>();
        }

        protected T Acquire<T>() where T : class, IInit
        {
            return _facade.Acquire<T>();
        }

        #endregion
    }
}

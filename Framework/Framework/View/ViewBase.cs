using System;
using System.Collections.Generic;
using Framework.Core.Interface;
using Framework.Core.Message;
using Framework.Core.Utility;
using Framework.Managers;
using UnityEngine;

namespace Framework.Core
{
    public interface IView : IMessage
    {
        bool IsShow { get; }
        void InitComponents();
        void Init(GameObject obj, IViewManage viewManage);
        void Show();
        void Hide();
        void Close();
        void UnsubscribeAll();
    }
    /// <summary>
    /// 界面基类
    /// </summary>
    public abstract class ViewBase : IView
    {
        protected GameObject ViewObj;
        private Facade _facade;
        private MessageManager _message;
        private readonly List<IUnsubscribeOnDestroy> _unsubscribeList = new List<IUnsubscribeOnDestroy>();
        private IViewManage _viewmanager;

        public bool IsShow => ViewObj.activeSelf;
        private ViewLayer _viewLayer = ViewLayer.Common;

        public ViewBase()
        {

        }
        public ViewBase(ViewLayer layer)
        {
            _viewLayer = layer;
        }

        protected LogUtility Log;
        public void Init(GameObject obj, IViewManage viewManage)
        {
            viewManage.SetPanelParent(obj, _viewLayer);
            _viewmanager = viewManage;
            ViewObj = obj;
            _facade = Facade.Inst;
            Log = GetUtility<LogUtility>();
            _message = GetManager<MessageManager>();
            InitComponents();
            OnInit();
            OnSubscribe();
        }

        public virtual void InitComponents()
        {

        }

        protected virtual void OnSubscribe()
        {

        }

        protected virtual void OnUnsubscribe()
        {

        }
        protected virtual void OnInit()
        {

        }

        public void Close()
        {
            _viewmanager.RemoveViewFromDict(this);
            OnUnsubscribe();
            UnsubscribeAll();
            OnClose();
        }

        public void Show(bool isShow)
        {
            if (ViewObj == null)
                return;

            ViewObj.SetActive(isShow);
        }



        protected virtual void OnClose()
        {
            UnityEngine.Object.Destroy(ViewObj);
            ViewObj = null;
        }


        public void UnsubscribeAll()
        {
            foreach (var unsubscribeOnDestroy in _unsubscribeList)
            {
                unsubscribeOnDestroy.UnsubscribeOnDestroy();
            }
            _unsubscribeList.Clear();
        }

        #region 显示隐藏

        public void Show()
        {
            if (ViewObj == null)
                return;

            ViewObj.SetActive(true);
            OnShow();
        }

        protected virtual void OnShow()
        {

        }

        public void Hide()
        {
            if (ViewObj == null)
                return;

            ViewObj.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide()
        {

        }

        #endregion

        #region 消息
        public void Publish<T>(T t) where T : IMessageBase
        {
            _message.Publish(t);
        }

        public void Publish<T>() where T : IMessageBase, new()
        {
            _message.Publish<T>();
        }


        public IUnsubscribeOnDestroy Subscribe<T>(Action<T> func) where T : IMessageBase
        {
            var unsubscribe = _message.Subscribe(func);
            _unsubscribeList.Add(unsubscribe);
            return unsubscribe;
        }

        public void Unsubscribe<T>(Action<T> func) where T : IMessageBase
        {
            _message.Unsubscribe(func);
        }

        #endregion

        #region 获取
        protected T GetManager<T>() where T : class, IManager, new()
        {
            return _facade.GetManager<T>();
        }

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return _facade.GetUtility<T>();
        }

        protected T GetModel<T>() where T : class, IModel, new()
        {
            return _facade.GetModel<T>();
        }

        protected T Acquire<T>() where T : class, IInit
        {
            return _facade.Acquire<T>();
        }

        
        #endregion

    }
}

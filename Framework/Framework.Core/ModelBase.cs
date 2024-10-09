using Framework.Core.Interface;
using Framework.Core.Message;

namespace Framework.Core
{
    /// <summary>
    /// 数据类 只提供数据
    /// </summary>
    public abstract class ModelBase : IModel
    {
        private IFacade _facade;
        private MessageManager _message;
        public void Init()
        {
            _facade = Facade.Inst;
            _message = _facade.GetManager<MessageManager>();
            OnInit();
        }

        protected virtual void OnInit()
        {

        }
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }

        protected void Publish<T>(T t) where T : IMessageBase
        {
            _message.Publish(t);
        }

        protected void Publish<T>() where T : IMessageBase, new()
        {
            _message.Publish<T>();
        }

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return _facade.GetUtility<T>();
        }
    }
}

using Framework.Core.Interface;

namespace Framework.Core
{
    /// <summary>
    /// 服务类 提供网络请求的数据或者本地数据
    /// </summary>
    public abstract class ServiceBase : IService
    {
        private IFacade _facade;
        public void Init()
        {
            _facade = Facade.Inst;
            OnInit();
        }


        protected virtual void OnInit()
        {

        }


        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            OnDispose();
        }

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return _facade.GetUtility<T>();
        }

        protected T GetService<T>() where T : class, IService, new()
        {
            return _facade.GetService<T>();
        }
    }
}

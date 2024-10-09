using Framework.Core.Interface;
namespace Framework.Core
{
    /// <summary>
    /// 管理类
    /// </summary>
    public abstract class ManagerBase : IManager
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

        protected T GetUtility<T>() where T : class, IUtility, new()
        {
            return _facade.GetUtility<T>();
        }

        protected T GetManager<T>() where T : class, IManager, new()
        {
            return _facade.GetManager<T>();
        }
    }
}

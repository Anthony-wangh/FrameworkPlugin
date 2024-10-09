using Framework.Core.Interface;

namespace Framework.Core
{
    /// <summary>
    /// 工具类 不与具体逻辑交互
    /// </summary>
    public abstract class UtilityBase : IUtility
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
    }
}

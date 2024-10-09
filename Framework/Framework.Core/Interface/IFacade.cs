namespace Framework.Core.Interface
{
    internal interface IFacade
    {
        void Bind<T>(T obj) where T : class, IInit;
        void UnRegister<T>() where T : class, IControl;
        void Register<T>() where T : class, IControl, new();
        T GetService<T>() where T : class, IService, new();
        T GetManager<T>() where T : class, IManager, new();
        T GetModel<T>() where T : class, IModel, new();
        T GetUtility<T>() where T : class, IUtility, new();
        T Acquire<T>() where T : class, IInit;
    }
}

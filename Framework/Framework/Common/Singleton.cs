
/// <summary>
/// 单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class, new()
{
    private static T _instance;

    public static T Inst => _instance ?? (_instance = new T());

    protected Singleton()
    {
    }

    /// <summary>
    /// 修改父类的单例 pc版本需要使用 一般不调用
    /// </summary>
    /// <param name="t"></param>
    public void SetInstance(T t)
    {
        _instance = t;
    }
}

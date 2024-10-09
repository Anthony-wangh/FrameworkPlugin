using UnityEngine;

/// <summary>
/// 带周期函数的单例
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    // ReSharper disable once StaticMemberInGenericType
    private static bool _isInitialized;
    /// <summary>
    /// 单例
    /// </summary>
    public static T Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    if (_instance == null)
                    {
                        Debug.LogError("Problem during the creation of " + typeof(T));
                    }
                }
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    if (_instance != null)
                        _instance.Init();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Debug.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
            DestroyImmediate(this);
            return;
        }
        if (!_isInitialized)
        {
            DontDestroyOnLoad(gameObject);
            _isInitialized = true;
            if (_instance != null)
                _instance.Init();
        }
    }

    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
        }
        _instance = null;
    }

    public virtual void Init() { }
}

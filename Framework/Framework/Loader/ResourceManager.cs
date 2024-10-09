using Framework.Tools.Loader;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 动态加载
/// </summary>
public sealed class ResourceManager
{
    public const string ViewPathKey = "ViewLoader";
    public const string ViewUIPathKey = "ViewUILoader";
    public const string PrefabsPathKey = "PrefabsLoader";

    private static string _viewPrefabPath = "View";
    private static string _viewUIPath = "ViewAssets";
    private static string _prefabsPath = "Prefabs";

    private static ResourceLoader _loader;
    private static ResourceSyncLoader _syncLoader;

    static ResourceManager()
    {
        _loader = new ResourceLoader();
        _syncLoader = new ResourceSyncLoader();
    }

    public static void Init()
    {
        _viewPrefabPath = PlayerPrefs.GetString(ViewPathKey, _viewPrefabPath);
        _viewUIPath = PlayerPrefs.GetString(ViewUIPathKey, _viewUIPath);
        _prefabsPath = PlayerPrefs.GetString(PrefabsPathKey, _prefabsPath);
    }

    #region Resources的加载

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"> 资源类型为文件夹名字，name为该文件夹下的文件名</param>
    /// <returns></returns>
    public static T LoadAsset<T>(string name) where T : Object
    {
        return _loader.LoadAsset<T>(name);
    }

    /// <summary>
    /// 加载预制
    /// </summary>
    /// <param name="name">Prefabs下面的预制</param>
    /// <returns></returns>
    public static GameObject LoadPrefabs(string name)
    {
        return _loader.LoadPrefabs(_prefabsPath + "/" + name);
    }


    /// <summary>
    /// 加载界面
    /// </summary>
    /// <param name="name">界面预制名字</param>
    /// <returns></returns>
    public static GameObject LoadView(string name)
    {
        return _loader.LoadView(_viewPrefabPath + "/" + name);
    }

    /// <summary>
    /// 加载view下面的UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">View/UI 之后的路径</param>
    /// <returns></returns>
    public static T LoadViewUI<T>(string name) where T : Object
    {
        return _loader.LoadViewUI<T>(_viewUIPath + "/" + name);
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    public static void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
    #endregion

    #region Resources 异步加载

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"> 资源类型为文件夹名字，name为该文件夹下的文件名</param>
    /// <param name="finish"></param>
    /// <returns></returns>
    public static void LoadAssetSync<T>(string name, Action<T> finish) where T : Object
    {
        _syncLoader.LoadAsset(name, finish);
    }

    /// <summary>
    /// 加载预制
    /// </summary>
    /// <param name="name">Prefabs下面的预制</param>
    /// <param name="finish"></param>
    /// <returns></returns>
    public static void LoadPrefabSync(string name, Action<GameObject> finish)
    {
        _syncLoader.LoadPrefabs(_prefabsPath + "/" +name, finish);
    }

    /// <summary>
    /// 加载预制
    /// </summary>
    /// <param name="name">Prefabs下面的预制</param>
    /// <param name="finish"></param>
    /// <returns></returns>
    public static void LoadPrefabSyncWithInst(string name, Action<GameObject> finish)
    {
        _syncLoader.LoadPrefabsWithInst(_prefabsPath + "/" + name, finish);
    }

    /// <summary>
    /// 加载界面
    /// </summary>
    /// <param name="name">界面预制名字</param>
    /// <param name="finish"></param>
    /// <returns></returns>
    public static void LoadViewSync(string name, Action<GameObject> finish)
    {
        _syncLoader.LoadView(_viewPrefabPath + "/" + name, finish);
    }

    /// <summary>
    /// 加载view下面的UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">View/UI 之后的路径</param>
    /// <param name="finish"></param>
    /// <returns></returns>
    public static void LoadViewUiSync<T>(string name, Action<T> finish) where T : Object
    {
        _syncLoader.LoadViewUI(_viewUIPath + "/" + name, finish);
    }
    #endregion

}


using UnityEngine;

namespace Framework.Tools.Loader
{
    /// <summary>
    /// Resource加载
    /// </summary>
    public class ResourceLoader
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T Load<T>(string path) where T : Object
        {
            // Unity版本2019.4.10f1，在LateUpdate中使用Resources.Load，有大概率会出现程序未响应的情况（Windows下）
            // 此Log保留至升级至其他版本
            // Debug.Log($"Resource.Load(\"{path}\")");
            var ret = Resources.Load<T>(path);
            // Debug.Log("LoadRet = "  + ret);
            return ret;
        }



        /// <summary>
        /// 加载界面
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject LoadView(string name)
        {
            GameObject obj = Load<GameObject>(name);
            if (obj == null)
            {
                Debug.Log("加载界面失败:" + name);
                return null;
            }

            return Instantiate(obj);
        }

        /// <summary>
        /// 加载界面UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T LoadViewUI<T>(string name) where T : Object
        {
            return Load<T>(name);
        }

        public GameObject LoadPrefabs(string path)
        {
            GameObject obj = Load<GameObject>(path);
            if (obj == null)
            {
                Debug.Log("加载Prefabs失败:" + path);
                return null;
            }

            return Instantiate(obj);
        }

        public T LoadAsset<T>(string name) where T : Object
        {
            string path = typeof(T).ToString();
            string[] str = path.Split('.');
            path = str[str.Length - 1];
            return Load<T>(path + "/" + name);
        }

        private GameObject Instantiate(GameObject obj)
        {
            if (obj == null)
            {
                Debug.Log("实例化对象为空");
                return null;
            }

            return Object.Instantiate(obj) as GameObject;
        }
    }
}

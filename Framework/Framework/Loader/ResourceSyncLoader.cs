using Framework.Common;
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Tools.Loader
{
    /// <summary>
    /// Resource异步加载
    /// </summary>
    public class ResourceSyncLoader
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        private void LoadSync<T>(string path, Action<T> finish) where T : UnityEngine.Object
        {
            CoreEngine.Inst.StartCoroutine(Load(path, finish));
        }

        private IEnumerator Load<T>(string path, Action<T> finish) where T : UnityEngine.Object
        {
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;
            if (rr.asset == null)
            {
                finish?.Invoke(null);
                yield break;
            }

            var result = rr.asset as T;
            if (result == null)
            {
                Debug.Log("加载失败:" + path);
            }
            finish?.Invoke(result);
        }


        /// <summary>
        /// 加载界面
        /// </summary>
        /// <param name="name"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public void LoadView(string name, Action<GameObject> finish)
        {
            CoreEngine.Inst.StartCoroutine(Load<GameObject>(name, (obj) =>
            {
                finish?.Invoke(Instantiate(obj));
            }));
        }

        /// <summary>
        /// 加载界面UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public void LoadViewUI<T>(string name, Action<T> finish) where T : UnityEngine.Object
        {
            CoreEngine.Inst.StartCoroutine(Load(name, finish));
        }
        public void LoadPrefabs(string name, Action<GameObject> finish)
        {
            CoreEngine.Inst.StartCoroutine(Load<GameObject>(name, (obj) =>
            {
                finish?.Invoke(obj);
            }));

        }
        public void LoadPrefabsWithInst(string name, Action<GameObject> finish)
        {
            CoreEngine.Inst.StartCoroutine(Load<GameObject>(name, (obj) =>
            {
                finish?.Invoke(Instantiate(obj));
            }));

        }

        public void LoadAsset<T>(string name, Action<T> finish) where T : UnityEngine.Object
        {
            string path = typeof(T).ToString();
            string[] str = path.Split('.');
            path = str[str.Length - 1];
            CoreEngine.Inst.StartCoroutine(Load(path, finish));
        }

        private GameObject Instantiate(GameObject obj)
        {
            if (obj == null)
            {
                Debug.Log("实例化对象为空");
                return null;
            }

            return UnityEngine.Object.Instantiate(obj) as GameObject;
        }
    }
}

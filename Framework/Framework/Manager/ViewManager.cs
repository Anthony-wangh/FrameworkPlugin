using Framework.Core;
using Framework.Core.Interface;
using Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Managers
{
    public enum ViewLayer
    {
        Bottom,
        Common,
        Pop,
        Title,
    }
    public interface IViewManage : IManager
    {
        T OpenView<T>() where T : IView, new();
        void CloseView<T>() where T : IView;
        void RemoveViewFromDict(IView view);
        void SetPanelParent(GameObject panel, ViewLayer layer, bool worldPosStays = false);
    }
    /// <summary>
    /// 界面管理
    /// </summary>
    public sealed class ViewManager : ManagerBase, IViewManage
    {
        /// <summary>
        /// 界面字典
        /// </summary>
        private readonly Dictionary<string, IView> _viewDict = new Dictionary<string, IView>();


        /// <summary>
        /// 层级父物体
        /// </summary>
        private readonly Dictionary<ViewLayer, Transform> LayerList = new Dictionary<ViewLayer, Transform>();

        /// <summary>
        /// 画布对象
        /// </summary>
        private GameObject CanvasObj;

        public Camera UiCamera { get; private set; }


        private const string TitleContainer = "Title";
        private const string MainContainer = "Main";

        private float TitleHeight = 80f;

        private GameObject _main;

        //private AssetBundle _viewAssetBundle;

        public static string PanelAssetsPath => Application.streamingAssetsPath + "/AssetBundles/view";

        public void InitCanvas(GameObject canvas, float topHeight)
        {
            CanvasObj = canvas;
            TitleHeight = topHeight;
        }
        protected override void OnInit()
        {
            InitUICamera();
            InitMainContainer();
            InitTitleContainer();
            InitLayer();
            //_viewAssetBundle = AssetBundle.LoadFromFile(PanelAssetsPath);
        }

        private void InitTitleContainer()
        {
            GameObject layer = new GameObject(TitleContainer);
            RectTransform rectrf = layer.AddComponent<RectTransform>();
            layer.layer = LayerMask.NameToLayer("UI");
            layer.transform.SetParent(CanvasObj.transform, false);
            rectrf.sizeDelta = Vector2.zero;
            rectrf.anchorMin = new Vector2(0, 1);
            rectrf.anchorMax = Vector2.one;
            rectrf.pivot = new Vector2(0.5f, 1);
            rectrf.SetAnchoredPosY(0);
            rectrf.SetSizeDeltaY(TitleHeight);
            rectrf.offsetMin = new Vector2(0, rectrf.offsetMin.y);
            rectrf.offsetMax = new Vector2(0, rectrf.offsetMax.y);
            LayerList.Add(ViewLayer.Title, layer.transform);
        }

        private void InitMainContainer()
        {
            _main = new GameObject(MainContainer);
            RectTransform rectrf = _main.AddComponent<RectTransform>();
            _main.layer = LayerMask.NameToLayer("UI");
            _main.transform.SetParent(CanvasObj.transform, false);
            rectrf.sizeDelta = Vector2.zero;
            rectrf.anchorMin = Vector2.zero;
            rectrf.anchorMax = Vector2.one;
            rectrf.offsetMin = Vector2.zero;
            rectrf.offsetMax = new Vector2(0, -TitleHeight);

            var y = rectrf.rect.height;
            rectrf.anchoredPosition = new Vector2(rectrf.anchoredPosition.x, y);
        }
        /// <summary>
        /// 设置为全屏（Title层向上移出屏幕，Main层全屏）
        /// </summary>
        public void SetAsFullScreen(bool isFull)
        {
            if (_main != null)
            {
                var main = _main.GetComponent<RectTransform>();
                main.offsetMax = new Vector2(0, isFull ? 0 : -TitleHeight);
            }

            if (LayerList.ContainsKey(ViewLayer.Title))
            {
                var title = LayerList[ViewLayer.Title].GetRectrf();
                title.SetAnchoredPosY(isFull ? TitleHeight : 0);
            }
        }
        /// <summary>
        /// 初始化层级
        /// </summary>
        private void InitLayer()
        {
            for (int i = 0; i < 3; i++)
            {
                ViewLayer vlayer = (ViewLayer)i;
                var layer = AddLayer(vlayer.ToString());
                LayerList.Add(vlayer, layer.transform);
            }
        }

        public GameObject AddLayer(string name)
        {
            GameObject layer = new GameObject(name);
            RectTransform rectrf = layer.AddComponent<RectTransform>();
            layer.layer = LayerMask.NameToLayer("UI");
            layer.transform.SetParent(_main.transform, false);
            rectrf.sizeDelta = Vector2.zero;
            rectrf.anchorMin = Vector2.zero;
            rectrf.anchorMax = Vector2.one;
            return layer;
        }

        private void InitUICamera()
        {
            var canvas = CanvasObj.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("找不到canvas");
                return;
            }

            UiCamera = canvas.worldCamera;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenView<T>() where T : IView, new()
        {
            string panelName = GetClassName<T>();
            if (_viewDict.ContainsKey(panelName))
            {
                T view = (T)_viewDict[panelName];
                view.Show();
                return (T)_viewDict[panelName];
            }

            //var panelObj = _viewAssetBundle.LoadAsset<GameObject>(panelName);
            //panelObj = Object.Instantiate(panelObj);
            //if (panelObj == null)
            //{
            //    Debug.Log("界面资源加载失败：" + panelName);
            //    return default(T);
            //}
            var panelObj = ResourceManager.LoadView(panelName);
            panelObj.SetLayer(LayerMask.NameToLayer("UI"));
            panelObj.name = panelName;
            var t = new T();
            _viewDict.Add(panelName, t);
            t.Init(panelObj,this);
            t.Show();
            return t;
        }

       

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseView<T>() where T : IView
        {
            string viewName = GetClassName<T>();
            if (!_viewDict.ContainsKey(viewName))
                return;
            _viewDict[viewName].Close();
        }
        /// <summary>
        /// 界面是否激活
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsShow<T>() where T : IView
        {
            string viewName = GetClassName<T>();
            if (!_viewDict.ContainsKey(viewName))
                return false;

            return _viewDict[viewName].IsShow;
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideView<T>() where T : IView
        {
            string viewName = GetClassName<T>();
            if (!_viewDict.ContainsKey(viewName))
                return;

            _viewDict[viewName].Hide();
        }

        public T GetView<T>() where T : IView
        {
            string viewName = GetClassName<T>();
            if (!_viewDict.ContainsKey(viewName))
                return default;

            return (T)_viewDict[viewName];
        }

        public void CloseAllView()
        {
            string[] viewNames = _viewDict.Keys.ToArray();
            foreach (var viewName in viewNames)
            {
                if (_viewDict.ContainsKey(viewName))
                {
                    _viewDict[viewName].Close();
                }
            }
            _viewDict.Clear();
        }

        public void CloseOtherView<T>() where T : IView
        {
            string viewName = GetClassName<T>();
            string[] names = _viewDict.Keys.ToArray();
            foreach (var name in names)
            {
                if (name == viewName)
                    continue;

                _viewDict[name].Close();
                _viewDict.Remove(name);
            }
        }


        public void RemoveViewFromDict(IView view)
        {
            string viewName = view.GetType().Name;
            if (!_viewDict.ContainsKey(viewName))
                return;

            _viewDict.Remove(viewName);
        }
        /// <summary>
        /// 设置层级父对象
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="layer"></param>
        /// <param name="worldPosStays"></param>
        public void SetPanelParent(GameObject panel, ViewLayer layer, bool worldPosStays = false)
        {
            panel.transform.SetParent(GetLayerParent(layer), worldPosStays);
        }

        public Transform GetLayerParent(ViewLayer layer)
        {
            if (!LayerList.ContainsKey(layer))
                return CanvasObj.transform;

            return LayerList[layer];
        }

        private string GetClassName<T>() where T : IView
        {
            string name = typeof(T).ToString();
            int lastIndex = name.LastIndexOf(".", StringComparison.Ordinal);
            if (lastIndex == -1)
                return name;

            return name.Substring(lastIndex + 1);
        }
        /// <summary>
        /// 获取所有界面
        /// </summary>
        /// <returns></returns>
        public List<IView> GetAllView()
        {
            if (_viewDict.Count == 0)
                return null;
            return _viewDict.Values.ToList();
        }

    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WXFramwork.Utility;

namespace WXFramework.UI
{
    public class UIWindow : IUIWindow
    {
        private int _windowId;
        private int _depth;
        private bool _isCover;
        private AUIBase _auiBase;
        private GameObject _gameObject;
        private List<Canvas> _cachedCanvasContainer;
        
        public UIWindow(int windowId)
        {
            _windowId = windowId;
            _depth = 0;
            _isCover = false;
            _auiBase = UIManager.Instance.GetUIBase(windowId);
            _cachedCanvasContainer = new List<Canvas>();
        }

        public int WindowId
        {
            get => _windowId;
        }

        public int Depth
        {
            get => _depth;
        }
        
        public bool IsCover
        {
            get => _isCover;
            set => _isCover = value;
        }

        public bool IsOtherCovered
        {
            get => _auiBase.IsOtherCovered;
        }

        public GameObject GameObject
        {
            get => _gameObject;
        }

        public void Update()
        {
            if (_gameObject != null && _auiBase != null)
            {
                _auiBase.OnUpdate();
            }
        }

        public void Load(Transform parent)
        {
            //资源加载先使用 AssetDatabase 同步加载，后续完善资源管理和加载模块
            GameObject gameObjectRes = AssetDatabase.LoadAssetAtPath<GameObject>(_auiBase.AssetPath);
            _gameObject = Object.Instantiate(gameObjectRes);
            RectTransform transform = _gameObject.GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
            transform.SetParent(parent);
            
            Canvas canvas = _gameObject.GetOrAddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 0;
            
            _gameObject.GetOrAddComponent<GraphicRaycaster>();
            
            DepthChanged(_depth);

            _auiBase.UIWindow = this;
            _auiBase.OnInit();
        }

        public void Cover()
        {
            _auiBase.OnHide();
            
            _gameObject.SetActive(false);
            
        }

        public void Reveal()
        {
            _auiBase.OnShow();
            
            _gameObject.transform.SetAsLastSibling();
            _gameObject.SetActive(true);
        }

        public void Destroy()
        {
            _auiBase.OnDestroy();
            //卸载资源
            Object.Destroy(_gameObject);
        }

        public void DepthChanged(int depth)
        {
            //游戏对象层级深度设置
            _depth = depth;
            _gameObject.GetComponentsInChildren(true, _cachedCanvasContainer);
            for (int i = 0; i < _cachedCanvasContainer.Count; i++)
            {
                _cachedCanvasContainer[i].sortingOrder = depth + i;
            }

            _cachedCanvasContainer.Clear();
        }
    }
}
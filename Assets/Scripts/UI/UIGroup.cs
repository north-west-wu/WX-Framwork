using System.Collections.Generic;
using UnityEngine;

namespace WXFramwork.UI
{
    public class UIGroup : IUIGroup
    {
        private string _groupName;
        private int _depth;
        private Canvas _canvas;
        private bool _pause;
        private List<IUIWindow> _uiWindows;

        public UIGroup(Canvas canvas, string name, int depth)
        {
            _canvas = canvas;
            _groupName = name;
            _depth = depth;
            _uiWindows = new List<IUIWindow>();
            
            _canvas.gameObject.name = name;
            RectTransform transform = _canvas.GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
            RefreshDepth();
        }

        public string GroupName
        {
            get => _groupName;
        }

        public int Depth
        {
            get => _depth;
            set
            {
                if (_depth == value)
                {
                    return;
                }

                _depth = value;
                RefreshDepth();
                //刷新
                Refresh();
            }
        }

        public bool Pause
        {
            get => _pause;
            set
            {
                if (_pause == value)
                {
                    return;
                }

                _pause = value;
                //刷新
                Refresh();
            }
        }
        
        public int UIFormCount { get; }
        
        public IUIWindow CurrentUIWindow { get; }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// 是否存在该面板
        /// </summary>
        public bool HasUIWindow(int uiWindowId)
        {
            foreach (IUIWindow uiWindow in _uiWindows)
            {
                if (uiWindow.WindowId == uiWindowId)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 进入 UI 界面
        /// </summary>
        public void PushUIWindow(int uiWindowId)
        {
            if (!HasUIWindow(uiWindowId))
            {
                Debug.LogError("当前UI界面已存在，无需添加");
                return;
            }
            
            //创建 UIWindow
            IUIWindow uiWindow = null;
            
            //进栈，但不直接刷新
            _uiWindows.Add(uiWindow);
        }
        
        /// <summary>
        /// 移除 UI 界面
        /// </summary>
        public void PopUIWindow(int uiWindowId)
        {
            IUIWindow uiWindow = GetUIWindow(uiWindowId);
            if (uiWindow == null)
            {
                return;
            }
            
            //界面关闭
            uiWindow.Close();
        }

        public IUIWindow GetUIWindow(int uiWindowId)
        {
            foreach (IUIWindow uiWindow in _uiWindows)
            {
                if (uiWindow.WindowId == uiWindowId)
                {
                    return uiWindow;
                }
            }

            return null;
        }

        public IUIWindow[] GetAllUIWindows()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 刷新界面组。
        /// </summary>
        public void Refresh()
        {
            bool pause = _pause;
        }
        
        /// <summary>
        /// 刷新组的深度
        /// </summary>
        private void RefreshDepth()
        {
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _depth * 10000;
        }
    }
}
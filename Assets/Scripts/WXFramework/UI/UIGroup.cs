using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WXFramwork.Utility;

namespace WXFramework.UI
{
    public class UIGroup : IUIGroup
    {
        private string _groupName;
        private int _depth;
        private GameObject _gameObject;
        private bool _cover;
        private List<IUIWindow> _uiWindows;

        public UIGroup(GameObject gameObject, string name, int depth)
        {
            _gameObject = gameObject;
            _groupName = name;
            _depth = depth;
            _uiWindows = new List<IUIWindow>();
            
            _gameObject.name = name;
            RectTransform transform = _gameObject.GetComponent<RectTransform>();
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
        
        /// <summary>
        /// 组是否被覆盖（隐藏）
        /// </summary>
        public bool Cover
        {
            get => _cover;
            set
            {
                if (_cover == value)
                {
                    return;
                }

                _cover = value;
                //刷新
                Refresh();
            }
        }

        public int UIFormCount
        {
            get => _uiWindows.Count;
        }

        public IUIWindow CurrentUIWindow
        {
            get => _uiWindows.Last();
        }

        public void Update()
        {
            for (int i = 0; i < _uiWindows.Count; i++)
            {
                IUIWindow uiWindow = _uiWindows[i];
                if (!uiWindow.IsCover)
                {
                    uiWindow.Update();
                }
            }
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
            if (HasUIWindow(uiWindowId))
            {
                Debug.LogError("当前UI界面已存在，无需添加");
                return;
            }
            
            //创建 UIWindow
            IUIWindow uiWindow = new UIWindow(uiWindowId);
            
            //加载 UIWindow
            uiWindow.Load(_gameObject.transform);
            
            //加入列表
            _uiWindows.Add(uiWindow);
            
            //显示
            uiWindow.Reveal();
            
            //刷新
            Refresh();
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
            
            //移除
            _uiWindows.Remove(uiWindow);
            //界面销毁
            uiWindow.Destroy();
            //刷新
            Refresh();
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
            return _uiWindows.ToArray();
        }
        
        /// <summary>
        /// 移除所有 UIWindow
        /// </summary>
        public void RemoveAllUIWindows()
        {
            for (int i = 0; i < _uiWindows.Count; i++)
            {
                _uiWindows[i].Destroy();
            }
            
            _uiWindows.Clear();
        }
        
        /// <summary>
        /// 激活对应面板
        /// </summary>
        public void RefocusUIWindow(int uiWindowId)
        {
            int index = -1;
            for (int i = 0; i < _uiWindows.Count; i++)
            {
                if (_uiWindows[i].WindowId == uiWindowId)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"未找到 UIWindowId：{uiWindowId} 的界面");
                return;
            }
            
            //交换位置
            IUIWindow temp = _uiWindows[_uiWindows.Count - 1];
            _uiWindows[_uiWindows.Count - 1] = _uiWindows[index];
            _uiWindows[index] = temp;
            //刷新
            Refresh();
        }

        /// <summary>
        /// 刷新界面组。
        /// </summary>
        public void Refresh()
        {
            if (_uiWindows.Count <= 0)
            {
                return;
            }
            
            bool cover = _cover;
            for (int i = _uiWindows.Count - 1; i >= 0; i--)
            {
                IUIWindow uiWindow = _uiWindows[i];
                uiWindow.DepthChanged(Depth * 10000 + (i + 1) * 100);
                //组覆盖状态
                if (cover)
                {
                    if (!uiWindow.IsCover)
                    {
                        uiWindow.IsCover = true;
                        uiWindow.Cover();
                    }
                }
                //组非覆盖状态
                else
                {
                    if (uiWindow.IsCover)
                    {
                        uiWindow.IsCover = false;
                        uiWindow.Reveal();
                    }

                    if (uiWindow.IsOtherCovered)
                    {
                        cover = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// 刷新组的深度
        /// </summary>
        private void RefreshDepth()
        {
            Canvas canvas = _gameObject.GetOrAddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = _depth * 10000;
        }
    }
}
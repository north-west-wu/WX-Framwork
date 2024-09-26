using System;
using System.Collections.Generic;
using UnityEngine;
using WXFramework.Core;

namespace WXFramework.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private readonly Dictionary<int, IUIBase> _allUiBases = new();
        private readonly Dictionary<int, IUIGroup> _uiGroups = new();
        
        protected override void Awake()
        {
            CodeTypes codeTypes = CodeTypes.Instance;
            foreach (Type type in codeTypes.GetTypes(typeof (UIBaseAttribute)))
            {
                IUIBase obj = Activator.CreateInstance(type) as IUIBase;
                if (obj == null)
                {
                    throw new Exception($"type not is AUIBase: {type.Name}");
                }
                
                object[] attrs = type.GetCustomAttributes(typeof(UIBaseAttribute), false);
                foreach (object attr in attrs)
                {
                    UIBaseAttribute uiBaseAttribute = attr as UIBaseAttribute;

                    if (uiBaseAttribute == null)
                    {
                        throw new Exception($"当前类型 {type.Name} 有除了 UIBase 特性以外的其他特性，需要删除");
                    }

                    int windowId = uiBaseAttribute.WindowId;
                    obj.WindowId = windowId;
                    if (!_allUiBases.ContainsKey(windowId))
                    {
                        _allUiBases.Add(windowId, obj);
                    }
                    else
                    {
                        throw new Exception($"WindowID: {windowId} 只能匹配一个对象，请检查代码是否出现问题");
                    }
                }
            }
        }

        public override void Update()
        {
        }
        
        /// <summary>
        /// 获取 UIBase
        /// </summary>
        /// <param name="windowId"> WindowID </param>
        public AUIBase GetUIBase(int windowId)
        {
            if (!_allUiBases.TryGetValue(windowId, out IUIBase iuiBase))
            {
                Debug.LogError($"未检查到 WindowId : {windowId} 有被注册的记录，请检查是否已添加对应特性到对应类上");
                return null;
            }
            
            return iuiBase as AUIBase;
        }
        
        /// <summary>
        /// 是否存在 UIGroup
        /// </summary>
        public bool HasUIGroup(int uiGroupId)
        {
            return _uiGroups.ContainsKey(uiGroupId);
        }
        
        /// <summary>
        /// 添加 UI Group
        /// </summary>
        public bool AddUIGroup(GameObject go, int uiGroupId)
        {
            if (HasUIGroup(uiGroupId))
            {
                Debug.Log($"已存在 Group : {uiGroupId}，无需添加");
                return false;
            }
            
            _uiGroups.Add(uiGroupId, new UIGroup(go, go.name, uiGroupId));
            return true;
        }
        
        /// <summary>
        /// 移除 UI Group
        /// </summary>
        public bool RemoveGroup(int uiGroupId)
        {
            if (!HasUIGroup(uiGroupId))
            {
                Debug.LogError($"不存在 Group : {uiGroupId}，无需移除");
                return false;
            }

            _uiGroups[uiGroupId].RemoveAllUIWindows();
            _uiGroups.Remove(uiGroupId);
            return true;
        }
        
        /// <summary>
        /// 打开 UI 界面
        /// </summary>
        /// <param name="uiGroupId">组Id</param>
        /// <param name="uiWindowId">界面Id</param>
        public void OpenUIWindow(int uiGroupId, int uiWindowId)
        {
            if (!HasUIGroup(uiGroupId))
            {
                Debug.LogError($"不存在 Group : {uiGroupId}，无需移除");
                return;
            }

            IUIGroup uiGroup = _uiGroups[uiGroupId];
            uiGroup.PushUIWindow(uiWindowId);
        }
        
        /// <summary>
        /// 关闭 UI 界面
        /// </summary>
        /// <param name="uiGroupId">组Id</param>
        /// <param name="uiWindowId">界面Id</param>
        public void CloseUIWindow(int uiGroupId, int uiWindowId)
        {
            if (!HasUIGroup(uiGroupId))
            {
                Debug.LogError($"不存在 Group : {uiGroupId}，无需移除");
                return;
            }

            IUIGroup uiGroup = _uiGroups[uiGroupId];
            uiGroup.PopUIWindow(uiWindowId);
        }

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiGroupId">组Id</param>
        /// <param name="uiWindowId">界面Id</param>
        public void RefocusUIWindow(int uiGroupId, int uiWindowId)
        {
            if (!HasUIGroup(uiGroupId))
            {
                Debug.LogError($"不存在 Group : {uiGroupId}，无需移除");
                return;
            }

            IUIGroup uiGroup = _uiGroups[uiGroupId];
            uiGroup.RefocusUIWindow(uiWindowId);
        }
    }
}
using UnityEngine;

namespace WXFramwork.UI
{
    public interface IUIGroup
    {
        /// <summary>
        /// 界面组名称。
        /// </summary>
        string GroupName
        {
            get;
        }
        
        /// <summary>
        /// 获取或设置界面组深度。
        /// </summary>
        int Depth
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取或设置界面组是否暂停。
        /// </summary>
        bool Pause
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取界面组中界面数量。
        /// </summary>
        int UIFormCount
        {
            get;
        }
        
        /// <summary>
        /// 获取当前界面。
        /// </summary>
        IUIWindow CurrentUIWindow
        {
            get;
        }

        void Update();
        
        /// <summary>
        /// 界面组中是否存在界面。
        /// </summary>
        bool HasUIWindow(int uiWindowId);
        
        /// <summary>
        /// 放入 UI 界面
        /// </summary>
        void PushUIWindow(int uiWindowId);
        
        /// <summary>
        /// 弹出 UI 界面
        /// </summary>
        void PopUIWindow(int uiWindowId);
        
        /// <summary>
        /// 从界面组中获取所有界面。
        /// </summary>
        /// <returns>界面组中的所有界面。</returns>
        IUIWindow[] GetAllUIWindows();
    }
}
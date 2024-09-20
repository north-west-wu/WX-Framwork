namespace WXFramwork.UI
{
    public interface IUIWindow
    {
        int WindowId
        {
            get;
        }
        
        /// <summary>
        /// 界面名字
        /// </summary>
        string WindowName
        {
            get;
        }
        
        /// <summary>
        /// 获取界面在界面组中的深度。
        /// </summary>
        int DepthInUIGroup
        {
            get;
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        void Update();
        
        /// <summary>
        /// 打开
        /// </summary>
        void Show();
        
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
        
        /// <summary>
        /// 界面深度改变。
        /// </summary>
        void DepthChanged(int uiGroupDepth, int depthInUIGroup);
    }
}
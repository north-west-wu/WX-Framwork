namespace WXFramework.UI
{
    public abstract class AUIBase : IUIBase
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public abstract string AssetPath { get; }
        
        /// <summary>
        /// 该界面是否会覆盖其他界面
        /// </summary>
        public abstract bool IsOtherCovered { get; }
        
        /// <summary>
        /// UIWindow
        /// </summary>
        public UIWindow UIWindow { get; set; }
        
        /// <summary>
        /// 更新
        /// </summary>
        public virtual void OnUpdate() { }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit() { }
        
        /// <summary>
        /// 显示
        /// </summary>
        public virtual void OnShow() { }
        
        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void OnHide() { }
        
        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnDestroy() { }
    }
}
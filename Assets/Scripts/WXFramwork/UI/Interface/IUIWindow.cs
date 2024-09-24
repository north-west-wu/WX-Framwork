using UnityEngine;

namespace WXFramwork.UI
{
    public interface IUIWindow
    {
        int WindowId
        {
            get;
        }
        
        /// <summary>
        /// 获取界面在界面组中的深度。
        /// </summary>
        int Depth
        {
            get;
        }
        
        /// <summary>
        /// 是否覆盖状态
        /// </summary>
        bool IsCover
        {
            get;
            set;
        }
        
        /// <summary>
        /// 是否覆盖的其他界面
        /// </summary>
        bool IsOtherCovered
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
        void Load(Transform parent);
        
        /// <summary>
        /// 覆盖
        /// </summary>
        void Cover();
        
        /// <summary>
        /// 覆盖恢复
        /// </summary>
        void Reveal();
        
        /// <summary>
        /// 销毁
        /// </summary>
        void Destroy();
        
        /// <summary>
        /// 界面深度改变。
        /// </summary>
        void DepthChanged(int depth);
    }
}
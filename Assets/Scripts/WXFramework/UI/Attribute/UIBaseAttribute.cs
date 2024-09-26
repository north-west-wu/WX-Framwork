using WXFramework.Core;

namespace WXFramework.UI
{
    /// <summary>
    /// UI 特性
    /// </summary>
    public class UIBaseAttribute : BaseAttribute
    {
        public int WindowId { get; }

        public UIBaseAttribute(int windowId)
        {
            WindowId = windowId;
        }
    }
}
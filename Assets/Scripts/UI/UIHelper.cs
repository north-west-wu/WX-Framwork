using System;

namespace WXFramwork.UI
{
    public class UIWindowId
    {
        public int UIMain = 1;
    }
    
    public class UIHelper
    {
        /// <summary>
        /// 获取资源加载路径
        /// </summary>
        /// <param name="uiWindowId"></param>
        /// <returns></returns>
        public string GetPath(int uiWindowId) =>
            uiWindowId switch
            {   
                1 => "",
                _ => throw new Exception($"没有对应 {uiWindowId} 的路径"),
            };
    }
}
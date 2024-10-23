using System.Collections.Generic;

namespace WXFramwork.Resource
{
    /// <summary>
    /// 一个资源组就是一个 AB 包
    /// </summary>
    public class ResGroup
    {
        /// <summary>
        /// 资源组路径
        /// </summary>
        public string GroupPath { get; set; }

        /// <summary>
        /// AB 包名称
        /// </summary>
        public string AssetBundleName { get; set; }
        
        /// <summary>
        /// 组Bundle里所有资源
        /// </summary>
        public List<string> FilePathList { get; set; }

        /// <summary>
        /// 依赖的文件的名字
        /// </summary>
        public List<string> DependFileName { get; set; }
        
        public ResGroup(string groupPath, string assetBundleName)
        {
            this.GroupPath = groupPath;
            this.AssetBundleName = assetBundleName;
            
            FilePathList = new List<string>();
            DependFileName = new List<string>();
        }
    }
}
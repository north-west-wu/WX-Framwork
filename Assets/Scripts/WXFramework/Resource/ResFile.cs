using System.Collections.Generic;

namespace WXFramwork.Resource
{
    public class ResFile
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// AB 包名称
        /// </summary>
        public string AssetBundleName { get;set; }
        
        /// <summary>
        /// 依赖的文件的名字
        /// </summary>
        public List<string> DependFileName { get; set; }
        
        public ResFile(string filePath, string abName)
        {
            this.FilePath = filePath;
            this.AssetBundleName = abName;

            DependFileName = new List<string>();
        }
    }
}
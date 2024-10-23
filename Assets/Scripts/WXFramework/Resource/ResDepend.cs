namespace WXFramwork.Resource
{
    public class ResDepend
    {
        /// <summary>
        /// 依赖路径
        /// </summary>
        public string DependPath { get; set; }

        /// <summary>
        /// AB 包名称
        /// </summary>
        public string AssetBundleName { get;set; }
        
        public ResDepend(string dependPath, string abName)
        {
            this.DependPath = dependPath;
            this.AssetBundleName = abName;
        }
    }
}
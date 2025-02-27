using System.Collections.Generic;
using UnityEngine;

namespace WXFramwork.Resource
{
    public class BundleRuntimeInfo
    {
        /// <summary>
        /// 分包的名称
        /// </summary>
        private string _bundlePackageName;
        
        /// <summary>
        /// 主动加载的文件
        /// </summary>
        internal readonly Dictionary<string, ResFile> ResFileDic = new Dictionary<string, ResFile>();
        
        /// <summary>
        /// 依赖加载的文件
        /// </summary>
        internal readonly Dictionary<string, ResDepend> ResDependDic = new Dictionary<string, ResDepend>();
        
        /// <summary>
        /// 分组的文件
        /// </summary>
        internal readonly Dictionary<string, ResGroup> ResGroupDic = new Dictionary<string, ResGroup>();
        
        /// <summary>
        /// 细分组的路径
        /// </summary>
        internal readonly List<string> ResGroupDicKey = new List<string>();
        
        /// <summary>
        /// Shader的AssetBundle
        /// </summary>
        internal AssetBundle Shader = null;
        
        public BundleRuntimeInfo(string bundlePackageName)
        {
            _bundlePackageName = bundlePackageName;
        }
    }
}
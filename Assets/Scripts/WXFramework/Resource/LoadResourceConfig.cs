using UnityEngine;

namespace WXFramwork.Resource
{
    public enum ResourceLoadMode
    {
        /// <summary>
        /// 开发模式(无需打包，编辑器下AssetDatabase加载)
        /// </summary>
        Develop = 0,
        
        /// <summary>
        /// 本地调试模式(需要打包，直接加载最新Bundle，不走热更逻辑)
        /// </summary>
        Local = 1,
        
        /// <summary>
        /// 发布模式(需要打包，走版本对比更新流程)
        /// </summary>
        Build = 2,
    }
    
    public class LoadResourceConfig
    {
        /// <summary>
        /// 存放本地Bundle的位置 Application.streamingAssetsPath;
        /// </summary>
        public string LocalBundlePath = Application.streamingAssetsPath;
        
        /// <summary>
        /// 资源服务器的地址
        /// </summary>
        public string BundleServerUrl = @"http://192.168.5.92/BundleData/";
        
        /// <summary>
        /// 资源热更新目录 Application.dataPath + "/../HotfixBundles/"
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public string HotfixPath = Application.dataPath + "/../HotfixBundles/";
#else
        public string HotfixPath = Application.persistentDataPath;
#endif
        
        /// <summary>
        /// 默认加载的Bundle名
        /// </summary>
        public string DefaultBundlePackageName = "";
        
        /// <summary>
        /// 加载模式
        /// </summary>
        public ResourceLoadMode ResourceLoadMode;
        
        /// <summary>
        /// 最大同时下载的资源数量
        /// </summary>
        public int MaxDownLoadCount = 3;

        /// <summary>
        /// 下载失败最多重试次数
        /// </summary>
        public int ReDownLoadCount = 8;

        public LoadResourceConfig(ResourceLoadMode mode)
        {
            this.ResourceLoadMode = mode;
        }
    }
}
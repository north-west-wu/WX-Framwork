using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace WXFramwork.Resource.Editor
{
    /// <summary>
    /// 配置资源包列表
    /// </summary>
    [CreateAssetMenu(fileName = "AssetBundleTable", menuName = "WXFramework/Resource/创建资源列表配置", order = 0)]
    public class AssetBundleTable : ScriptableObject
    {
        [Header("构建路径文件夹名称")]
        [Tooltip("构建的资源的相对路径(Assets同级目录下的路径)")] 
        public string BundlePath;
        
        [Header("初始场景")]
        [Tooltip("最后不打Bundle直接打进包体里的场景(Scene In Build 里填的场景)")] 
        public List<SceneAsset> InitScene;
        
        
        [FormerlySerializedAs("AssetsSettings")]
        [Header("所有分包配置信息")]
        [Tooltip("每一个分包的配置信息")]
        public List<AssetSubBundle> AssetsSubBundles;

        public string BuildBundlePath
        {
            get
            {
                string path = Path.Combine(Application.dataPath + "/../", BundlePath);
                DirectoryInfo info;
                if (!Directory.Exists(path))
                {
                    info = Directory.CreateDirectory(path);
                }
                else
                {
                    info = new DirectoryInfo(path);
                }
                return info.FullName;
            }  
        }
    }
    
    /// <summary>
    /// 配置资源分包
    /// </summary>
    [CreateAssetMenu(fileName = "AssetSubBundle", menuName = "WXFramework/Resource/创建资源分包配置", order = 1)]
    public class AssetSubBundle : ScriptableObject
    {
        [Header("分包名字")]
        [Tooltip("当前分包的包名")]
        public string BuildName;
        
        [Header("版本索引")]
        [Tooltip("表示当前Bundle的索引")]
        public int BuildIndex;
        
        [Header("AssetBundle的后缀")]
        [Tooltip("AssetBundle资源的的后缀名(如'bundle')")]
        public string BundleVariant;
        
        [Header("是否启用Hash名")]
        [Tooltip("是否使用Hash名替换Bundle名称")]
        public bool NameByHash;
        
        [Header("构建选项")]
        public BuildAssetBundleOptions BuildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;

        [Header("资源路径")]
        [Tooltip("需要打包的资源所在的路径(不需要包含依赖, 只包括需要主动加载的资源)")]
        public List<string> AssetPaths;
        
        [Header("组资源路径")]
        [Tooltip("资源颗粒控制")]
        public List<string> AssetGroupPaths;
        
        [Header("场景资源")]
        [Tooltip("需要通过Bundle加载的场景")]
        public List<SceneAsset> SceneAssets;
    }
}
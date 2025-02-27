using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WXFramwork.Resource
{
    public partial class ResourceManager
    {
        internal string BundleFileExistPath(string bundlePackageName, string fileName, bool isWebLoad)
        {
            string path = null;
            if (_config.ResourceLoadMode == ResourceLoadMode.Local)
            {
                // localPath/bundlePackName/fileName
                path = Path.Combine(_config.LocalBundlePath, bundlePackageName, fileName);
            }
            else
            {
                // hotfixPath/bundlePackName/fileName
                path = Path.Combine(_config.HotfixPath, bundlePackageName, fileName);
                if (!File.Exists(path))
                {
                    //热更目录不存在，返回本地目录
                    //localPath/bundlePackName/fileName
                    path = Path.Combine(_config.LocalBundlePath, bundlePackageName, fileName);
                }
            }
            
            //如果使用web加载的方式，需要加上 file 协议
            if (isWebLoad)
            {
                //通过webReq加载
                path = "file://" + path;
            }
            
            return path;
        }
        
        /// <summary>
        /// 获取分包的更新索引列表
        /// </summary>
        private async Task<string> GetRemoteBundlePackageVersionLog(string bundlePackageName)
        {
            byte[] data = await DownloadBundleTool.DownloadDataAsync(
                Path.Combine(_config.BundleServerUrl, bundlePackageName, "VersionLogs.txt"));
            if (data == null)
            {
                Debug.LogError(bundlePackageName + "获取更新索引列表失败");
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }
        
        /// <summary>
        /// 创建更新后的Log文件
        /// </summary>
        /// <param name="filePath">文件的全路径</param>
        /// <param name="fileData">文件的内容</param>
        private static void CreateUpdateLogFile(string filePath, string fileData)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(fileData);
                sw.WriteLine(sb.ToString());
            }
        }
    }
}
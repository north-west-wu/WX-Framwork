using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace WXFramwork.Resource
{
    public partial class ResourceManager
    {
        /// <summary>
        /// Bundle初始化的信息
        /// </summary>
        internal static readonly Dictionary<string, BundleRuntimeInfo> BundleNameToRuntimeInfo = new Dictionary<string, BundleRuntimeInfo>();
        
        public async Task<bool> Initialize(string bundlePackageName)
        {
            if (_config.ResourceLoadMode == ResourceLoadMode.Develop)
            {
                UnityEngine.Debug.Log("AssetLoadMode = Develop 不需要初始化Bundle配置文件");
                return false;
            }
            
            if (BundleNameToRuntimeInfo.ContainsKey(bundlePackageName))
            {
                UnityEngine.Debug.Log(bundlePackageName + " 重复初始化");
                return false;
            }
            
            BundleRuntimeInfo bundleRuntimeInfo = new BundleRuntimeInfo(bundlePackageName);
            BundleNameToRuntimeInfo.Add(bundlePackageName, bundleRuntimeInfo);
            
            //获取文件信息
            string filePath = BundleFileExistPath(bundlePackageName, "FileLogs.txt", true);
            TaskCompletionSource<bool> fileTcs = new TaskCompletionSource<bool>();
            using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath))
            {
                UnityWebRequestAsyncOperation reqOperation = webRequest.SendWebRequest();
                reqOperation.completed += o =>
                {
                    fileTcs.SetResult(true);
                };

                await fileTcs.Task;
                //是否获取成功
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("初始化分包未找到FileLogs 分包名: " + bundlePackageName + "\t" + filePath);
                    return false;
                }
                
                string fileLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(fileLogs);
                List<string> dependFileName = new List<string>();
                foreach (Match m in matchCollection)
                {
                    string[] fileLog = m.Groups[1].Value.Split('|');
                    ResFile resFile = new ResFile(fileLog[0], fileLog[1]);
                    
                    if (fileLog.Length > 2)
                    {
                        for (int i = 2; i < fileLog.Length; i++)
                        {
                            dependFileName.Add(fileLog[i]);
                        }
                    }
                    resFile.DependFileName = new List<string>(dependFileName);
                    dependFileName.Clear();
                    bundleRuntimeInfo.ResFileDic.Add(resFile.FilePath, resFile);
                }
            }
            
            //获取依赖信息
            TaskCompletionSource<bool> dependTcs = new TaskCompletionSource<bool>();
            string dependPath = BundleFileExistPath(bundlePackageName, "DependLogs.txt", true);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(dependPath))
            {
                UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                operation.completed += o =>
                {
                    dependTcs.SetResult(true);
                };

                await dependTcs.Task;
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("初始化分包未找到DependLogs 分包名: " + bundlePackageName + "\t" + dependPath);
                    return false;
                }
                
                string dependLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(dependLogs);
                foreach (Match m in matchCollection)
                {
                    string[] dependLog = m.Groups[1].Value.Split('|');
                    ResDepend resDepend = new ResDepend(dependLog[0], dependLog[1]);
                    bundleRuntimeInfo.ResDependDic.Add(resDepend.DependPath, resDepend);
                }
            }
            
            //获取组信息
            TaskCompletionSource<bool> groupTcs = new TaskCompletionSource<bool>();
            string groupPath = BundleFileExistPath(bundlePackageName, "GroupLogs.txt", true);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(groupPath))
            {
                UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
                operation.completed += o =>
                {
                    groupTcs.SetResult(true);
                };

                await groupTcs.Task;
                
                string groupLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(groupLogs);
                foreach (Match m in matchCollection)
                {
                    string[] groupLog = m.Groups[1].Value.Split('|');
                    ResGroup loadGroup = new ResGroup(groupLog[0], groupLog[1]);
                    if (groupLog.Length > 2)
                    {
                        for (int i = 2; i < groupLog.Length; i++)
                        {
                            loadGroup.DependFileName.Add(groupLog[i]);
                        }
                    }
                    bundleRuntimeInfo.ResGroupDic.Add(loadGroup.GroupPath, loadGroup);
                    bundleRuntimeInfo.ResGroupDicKey.Add(loadGroup.GroupPath);
                }
            }

            //加载当前分包的shader
            await LoadShader(bundlePackageName);
            return true;
        }
        
        /// <summary>
        /// 加载Shader文件
        /// </summary>
        private async Task LoadShader(string bundlePackageName)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            string shaderPath = BundleFileExistPath(bundlePackageName, "shader_" + bundlePackageName.ToLower(), true);
            byte[] shaderData = await VerifyTool.GetDataAsync(shaderPath);
            if (shaderData == null)
            {
                tcs.SetResult(true);
            }
            //内部文件加载
            else
            {
                shaderPath = BundleFileExistPath(bundlePackageName, "shader_" + bundlePackageName.ToLower(), false);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(shaderPath);
                request.completed += operation =>
                {
                    BundleNameToRuntimeInfo[bundlePackageName].Shader = request.assetBundle;
                    tcs.SetResult(true);
                };
            }
            
            await tcs.Task;
        }
    }
}
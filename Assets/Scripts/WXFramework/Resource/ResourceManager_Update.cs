using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace WXFramwork.Resource
{
    public partial class ResourceManager
    {
        /// <summary>
        /// 检查分包是否需要更新
        /// </summary>
        /// <param name="bundlePackageNames">所有分包的名称以及是否验证文件CRC</param>
        public async Task<UpdateBundleDataInfo> CheckAllBundlePackageUpdate(Dictionary<string, bool> bundlePackageNames)
        {
            UpdateBundleDataInfo updateBundleDataInfo = new UpdateBundleDataInfo();
            //不处于构建资源时，不检查资源
            if (_config.ResourceLoadMode != ResourceLoadMode.Build)
            {
                updateBundleDataInfo.NeedUpdate = false;
                if (_config.ResourceLoadMode == ResourceLoadMode.Local)
                {
                    _config.HotfixPath = _config.LocalBundlePath;
                }
                else
                {
#if !UNITY_EDITOR
                    AssetLogHelper.LogError("ResourceLoadMode = ResourceLoadMode.Develop 只能在编辑器下运行");
#endif
                }
                return updateBundleDataInfo;
            }
            
            //当前时区的时间
            updateBundleDataInfo.UpdateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            //开始处理每个分包的更新信息
            foreach (var bundlePackageInfo in bundlePackageNames)
            {
                string bundlePackageName = bundlePackageInfo.Key;
                //获取远端 Version Log
                string remoteVersionLog = await GetRemoteBundlePackageVersionLog(bundlePackageName);
                if (remoteVersionLog == null)
                {
                    Debug.LogError("未找到远程分包: " + bundlePackageName);
                    continue;
                }
                
                //创建各个分包对应的文件夹
                if (!Directory.Exists(Path.Combine(_config.HotfixPath, bundlePackageName)))
                {
                    Directory.CreateDirectory(Path.Combine(_config.HotfixPath, bundlePackageName));
                }
                
                //获取分包的 CRC Log
                string crcLogPath = Path.Combine(_config.HotfixPath, bundlePackageName, "CRCLog.txt");
                updateBundleDataInfo.PackageCRCDic.Add(bundlePackageName, new Dictionary<string, uint>());
                //如果存在 CRC 文件，则进行 CRC 对比，确认需要更新的文件
                if (File.Exists(crcLogPath))
                {
                    string crcLog;
                    using (StreamReader streamReader = new StreamReader(crcLogPath))
                    {
                        crcLog = await streamReader.ReadToEndAsync();
                    }
                    string[] crcLogData = crcLog.Split('\n');
                    for (int j = 0; j < crcLogData.Length; j++)
                    {
                        string crcLine = crcLogData[j];
                        if (string.IsNullOrWhiteSpace(crcLine))
                        {
                            continue;
                        }
                        string[] info = crcLine.Split('|');
                        if (info.Length != 3)
                        {
                            continue;
                        }
                        //abName|crc32|time
                        if (!uint.TryParse(info[1], out uint crc))
                        {
                            continue;
                        }
                        
                        //如果存在重复就覆盖
                        updateBundleDataInfo.PackageCRCDic[bundlePackageName][info[0]] = crc;
                    }
                }
                //没有则创建 CRC 文件
                else
                {
                    CreateUpdateLogFile(crcLogPath, null);
                }
                
                //把CRCLogPath的分包名存起来
                updateBundleDataInfo.PackageCRCFileDic.Add(bundlePackageName, null);
                
                //获取本地的Version Log
                string localVersionLogExistPath = BundleFileExistPath(bundlePackageName, "VersionLogs.txt", true);
                TaskCompletionSource<bool> logTcs = new TaskCompletionSource<bool>();
                string localVersionLog = null;
                using (UnityWebRequest webRequest = UnityWebRequest.Get(localVersionLogExistPath))
                {
                    UnityWebRequestAsyncOperation weq = webRequest.SendWebRequest();
                    weq.completed += (o) =>
                    {
                        logTcs.SetResult(true);
                    };
                    await logTcs.Task;
                    
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        localVersionLog = "INIT|0";
                    }
                    else
                    {
                        localVersionLog = webRequest.downloadHandler.text;
                    }
                }
                string[] remoteVersionData = remoteVersionLog.Split('\n');
                string[] localVersionData = localVersionLog.Split('\n');
                //如果需要更新，则进行更新数据
                if (bundlePackageInfo.Value)
                {
                    await CalcNeedUpdateBundleFileCRC(updateBundleDataInfo, bundlePackageName, remoteVersionData, localVersionData);
                }
                else
                {
                }
            }

            return updateBundleDataInfo;
        }

        /// <summary>
        /// 获取所有需要更新的Bundle的文件(计算文件CRC)
        /// </summary>
        private async Task CalcNeedUpdateBundleFileCRC(UpdateBundleDataInfo updateBundleDataInfo, string bundlePackageName, string[] remoteVersionData, string[] localVersionData)
        {
            //time/version/bool
            string[] remoteVersionDataSplits = remoteVersionData[0].Split('|');
            //版本号
            int remoteVersion = int.Parse(remoteVersionDataSplits[1]);
            //本地版本号
            int localVersion = int.Parse(localVersionData[0].Split('|')[1]);
            //添加版本号
            updateBundleDataInfo.PackageToVersion.Add(bundlePackageName, new int[2]{localVersion, remoteVersion});
            //资源类型判断，如加密资源，非加密资源，原生资源，这里先不处理，只处理非加密资源
            
            //版本异常进行提示
            if (localVersion > remoteVersion)
            {
                Debug.LogError("本地版本号优先与远程版本号 " + localVersion + ">" + remoteVersion + "\n"
                                        + "localBundleTime: " + localVersionData[0].Split('|')[0] + "\n"
                                        + "remoteBundleTime: " + remoteVersionData[0].Split('|')[0] + "\n"
                                        + "Note: 发送了版本回退或者忘了累进版本号");
            }

            int count = (remoteVersionData.Length - 1);
            
            // i * 5 + j + 1
            // 1 2 3 4 5
            // 6 7 8 9 10
        }
        
        /// <summary>
        /// 检测文件 CRC
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckFileCRC(string bundlePackageName, string remoteVersionDataLine)
        {
            //资源行是否为空
            if (!string.IsNullOrWhiteSpace(remoteVersionDataLine))
            {
                //path|fileLength|crc32
                string[] info = remoteVersionDataLine.Split('|');
                //如果文件不存在直接加入更新
                string filePath = BundleFileExistPath(bundlePackageName, info[0], true);
                uint fileCRC32 = await VerifyTool.GetFileCRC32(filePath);
                //判断是否和远程一样, 不一样直接加入更新
                if (uint.Parse(info[2]) != fileCRC32)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
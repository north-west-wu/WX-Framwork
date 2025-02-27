using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace WXFramwork.Resource
{
    public static class DownloadBundleTool
    {
        public static async Task<byte[]> DownloadDataAsync(string url, int reloadCount = 1)
        {
            for (int i = 0; i < reloadCount; i++)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
                    TaskCompletionSource<bool> waitDown = new TaskCompletionSource<bool>();
                    UnityWebRequestAsyncOperation webRequestAsync = webRequest.SendWebRequest();
                    webRequestAsync.completed += (asyncOperation) =>
                    {
                        waitDown.SetResult(true);
                    };
                    await waitDown.Task;
                
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        return webRequest.downloadHandler.data;
                    }
                }
            }

            Debug.LogError("下载资源失败: " + url);
            return null;
        }
    }
}
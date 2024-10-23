using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace WXFramwork.Resource.Editor
{
    public class BuildAssetTool
    {
        /// <summary>
        /// 得到一个路径下文件的大小
        /// </summary>
        public static long GetFileLength(string filePath)
        {
            return new FileInfo(filePath).Length;
        }
        
        /// <summary>
        /// 获取所有文件并过滤
        /// </summary>
        public static List<string> GetAllChildFilesAndFilter(string path)
        {
            List<string> files = new List<string>();
            GetAllChildFiles(path, files);
            return GetFilterFiles(files);
        }
        
        /// <summary>
        /// 获取所有子文件
        /// </summary>
        public static void GetAllChildFiles(string path, List<string> files)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            //获取该目录下的所有文件
            files.AddRange(directoryInfo.GetFiles().Select(file => path + "/" + file.Name));
            foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
            {
                GetAllChildFiles(subDirectoryInfo.FullName, files);
            }
        }

        public static List<string> GetFilterFiles(List<string> files)
        {
            List<string> filterFiles = new List<string>();
            for (int i = 0; i < files.Count; i++)
            {
                string path = files[i];
                
                if (CantLoadFile(path))
                {
                    continue;
                }
                string suffix = Path.GetExtension(path);
                switch (suffix)
                {
                    case ".dll":
                        continue;
                    case ".cs":
                        continue;
                    case ".meta":
                        continue;
                    case ".js":
                        continue;
                    case ".boo":
                        continue;
                }
                
                filterFiles.Add(path);
            }
            
            return filterFiles;
        }
        
        /// <summary>
        /// 不能加载的文件
        /// </summary>
        public static bool CantLoadFile(string fillPathOrName)
        {
            if (fillPathOrName.Contains("LightingData.asset"))
            {
                return true;
            }
            // if (fillPathOrName.Contains("Lightmap-"))
            // {
            //     return true;
            // }
            // if (fillPathOrName.Contains("ReflectionProbe-"))
            // {
            //     return true;
            // }
            return false;
        }
        
        /// <summary>
        /// 是Shader资源
        /// </summary>
        public static bool IsShaderAsset(string fileFullName)
        {
            string suffix = Path.GetExtension(fileFullName);
            switch (suffix)
            {
                case ".shader":
                    return true;
                case ".shadervariants":
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 是否是组文件
        /// </summary>
        public static bool IsGroupFile(string file, string groupPath)
        {
            //在文件路径中包含组路径即可
            if (file.Contains(groupPath))
            {
                return true;
            }

            return false;
        }
    }
}
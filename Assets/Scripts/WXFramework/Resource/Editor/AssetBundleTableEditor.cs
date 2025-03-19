using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace WXFramwork.Resource.Editor
{
    [CustomEditor(typeof(AssetBundleTable))]
    public class AssetBundleTableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space(20);
            if (GUILayout.Button("构建资源"))
            {
                BuildAssets();
            }
        }

        private void BuildAssets()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            AssetBundleTable assetBundleTable = serializedObject.targetObject as AssetBundleTable;
            //添加初始游戏场景，移除其他不需要的游戏场景
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (SceneAsset sceneAsset in assetBundleTable.InitScene)
            {
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(sceneAsset), true));
            }
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            
            //获取工程中使用的 Shader
            HashSet<string> alwaysIncludedShaders = new HashSet<string>();
            Object graphicsSettings = GraphicsSettings.GetGraphicsSettings();
            SerializedObject graphicsSettingsObject = new SerializedObject(graphicsSettings);
            SerializedProperty serializedProperty = graphicsSettingsObject.FindProperty("m_AlwaysIncludedShaders");
            if (serializedProperty.isArray)
            {
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    SerializedProperty property = serializedProperty.GetArrayElementAtIndex(i);
                    Object shaderInfo = property.objectReferenceValue;
                    if (!alwaysIncludedShaders.Contains(shaderInfo.name))
                    {
                        alwaysIncludedShaders.Add(shaderInfo.name);
                    }
                }
            }
            
            //所有子包
            List<AssetSubBundle> assetSubBundles = assetBundleTable.AssetsSubBundles;
            for (int i = 0; i < assetSubBundles.Count; i++)
            {
                AssetSubBundle subBundle = assetSubBundles[i];
                //构建 AB 包
                Build(assetBundleTable, subBundle, alwaysIncludedShaders);
            }
            
            AssetDatabase.SaveAssets();
            //打包结束
            sw.Stop();
            Debug.Log("打包结束, 耗时" + sw.Elapsed.TotalMilliseconds + " ms \n" + assetBundleTable.BundlePath);
        }

        private static void Build(AssetBundleTable bundleTable, AssetSubBundle subBundle, HashSet<string> alwaysIncludedShaders)
        {
            
        }
        
        /// <summary>
        /// AB 包名称
        /// </summary>
        private static string GetBundleName(AssetSubBundle subBundle, string filePath)
        {
            string bundlePackageName = subBundle.BuildName.ToLower();
            if (subBundle.NameByHash)
            {
                filePath = VerifyTool.GetMd5(bundlePackageName + filePath);
            }
            else
            {
                filePath = filePath.Replace("/", "_");
                filePath = filePath.Replace(".", "_");
                filePath = filePath.Replace(" ", "_");
                // filePath = filePath.Replace("+", "_");
                // filePath = filePath.Replace("-", "_");
                filePath = bundlePackageName + "_" + filePath;
                filePath = filePath.ToLower();
            }
            return filePath;
        }
        
        /// <summary>
        /// 创建AssetBundleBuild并添加管理
        /// </summary>
        private static void AddToAssetBundleBuilds(AssetSubBundle subBundle, List<AssetBundleBuild> assetBundleBuilds, HashSet<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                AssetBundleBuild abb = new AssetBundleBuild();
                abb.assetBundleName = GetBundleName(subBundle, filePath);
                abb.assetNames = new string[] { filePath };
                abb.assetBundleVariant = subBundle.BundleVariant;
                assetBundleBuilds.Add(abb);
            }
        }

        /// <summary>
        /// 保存加载用的Log
        /// </summary>
        private static void SaveLoadLog(AssetBundleTable bundleTable, AssetSubBundle subBundle,
            Dictionary<string, ResFile> resFiles, Dictionary<string, ResDepend> resDepends,
            Dictionary<string, ResGroup> resGroups)
        {
            //创建Log目录
            if (!Directory.Exists(Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName)))
            {
                Directory.CreateDirectory(Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName));
            }
            //写入文件Log
            using (StreamWriter sw = new StreamWriter(Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName, "FileLogs.txt")))
            {
                StringBuilder sb = new StringBuilder();
                //文件格式: <fileName|abName|depend|...>
                foreach (var resFile in resFiles)
                {
                    string data = "<" + resFile.Key + "|" + resFile.Value.AssetBundleName + "|";
                    foreach (string depend in resFile.Value.DependFileName)
                    {
                        data += depend + "|";
                    }
                    data = data.Substring(0, data.Length - 1);
                    data += ">" + "\n";
                    sb.Append(data);
                }
                sw.WriteLine(sb.ToString());
            }
            //写入依赖Log
            using (StreamWriter sw = new StreamWriter(Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName, "DependLogs.txt")))
            {
                StringBuilder sb = new StringBuilder();
                //依赖格式: <dependName|abName>
                foreach (var resDepend in resDepends)
                {
                    string data = "<" + resDepend.Key + "|" + resDepend.Value.AssetBundleName + ">\n";
                    sb.Append(data);
                }
                sw.WriteLine(sb.ToString());
            }
            //写入组Log
            using (StreamWriter sw = new StreamWriter(Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName, "GroupLogs.txt")))
            {
                StringBuilder sb = new StringBuilder();
                //组格式: <groupName|abName|depend|...>
                foreach (var resGroup in resGroups)
                {
                    string data = "<" + resGroup.Key + "|" + resGroup.Value.AssetBundleName + "|";
                    foreach (string depend in resGroup.Value.DependFileName)
                    {
                        data += depend + "|";
                    }
                    data = data.Substring(0, data.Length - 1);
                    data += ">" + "\n";
                    sb.Append(data);
                }
                sw.WriteLine(sb.ToString());
            }
        }
        
        /// <summary>
        /// 保存Bundle的版本号文件
        /// </summary>
        private static void SaveBundleVersionFile(string bundlePackagePath, AssetBundleManifest manifest, AssetSubBundle subBundle)
        {
            string[] assetBundles = manifest.GetAllAssetBundles();
            using (StreamWriter sw = new StreamWriter(Path.Combine(bundlePackagePath, "VersionLogs.txt")))
            {
                StringBuilder sb = new StringBuilder();
                //第一行：时间|版本号 
                string versionHandler = System.DateTime.Now + "|" + subBundle.BuildIndex + "\n";
                sb.Append(versionHandler);
                //ab包格式: abName|fileLength|crc32
                foreach (string assetBundle in assetBundles)
                {
                    string bundlePath = Path.Combine(bundlePackagePath, assetBundle);
                    uint crc32 = VerifyTool.GetCRC32(File.ReadAllBytes(bundlePath));
                    string info = assetBundle + "|" + BuildAssetTool.GetFileLength(bundlePath) + "|" + crc32 + "\n";
                    sb.Append(info);
                }
                sw.WriteLine(sb.ToString());
            }
        }
    }
}
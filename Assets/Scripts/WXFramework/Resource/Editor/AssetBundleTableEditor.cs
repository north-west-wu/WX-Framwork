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
            //记录所有加载路径
            HashSet<string> allAssetLoadPath = new HashSet<string>();
            for (int i = 0; i < assetSubBundles.Count; i++)
            {
                AssetSubBundle subBundle = assetSubBundles[i];
                //构建 AB 包
                Build(assetBundleTable, subBundle, allAssetLoadPath, alwaysIncludedShaders);
            }
            
            AssetDatabase.SaveAssets();
            //打包结束
            sw.Stop();
            Debug.Log("打包结束, 耗时" + sw.Elapsed.TotalMilliseconds + " ms \n" + assetBundleTable.BundlePath);
        }

        private static void Build(AssetBundleTable bundleTable, AssetSubBundle subBundle, HashSet<string> assetLoadPath, HashSet<string> alwaysIncludedShaders)
        {
            Dictionary<string, ResFile> resFiles = new Dictionary<string, ResFile>();
            Dictionary<string, ResDepend> resDepends = new Dictionary<string, ResDepend>();
            Dictionary<string, ResGroup> resGroups = new Dictionary<string, ResGroup>();
            
            //需要主动加载的文件的路径以及它的依赖bundle名字
            Dictionary<string, string[]> fileDepends = new Dictionary<string, string[]>();
            //被依赖的文件依赖的次数(依赖也是包含后缀的路径)
            Dictionary<string, int> dependenciesIndex = new Dictionary<string, int>();
            //组文件的依赖
            Dictionary<string, List<string>> groupFileToDepends = new Dictionary<string, List<string>>();
            
            //获取所有目录的所有子文件
            string[] paths = subBundle.AssetPaths.ToArray();
            //所有需要打包的文件
            HashSet<string> files = new HashSet<string>();
            //获取目录下的所有文件
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                //获取所有需要主动加载的资源
                List<string> filePaths = BuildAssetTool.GetAllChildFilesAndFilter(path);
                foreach (var filePath in filePaths)
                {
                    files.Add(filePath);
                }
            }
            //创建组包
            foreach (string assetGroupPath in subBundle.AssetGroupPaths)
            {
                ResGroup resGroup = new ResGroup(assetGroupPath, $"{GetBundleName(subBundle, assetGroupPath)}.{subBundle.BundleVariant}");
                resGroups.Add(assetGroupPath, resGroup);
            }
            
            //获取场景资源
            SceneAsset[] sceneAssets = subBundle.SceneAssets.ToArray();
            for (int i = 0; i < sceneAssets.Length; i++)
            {
                SceneAsset sceneAsset = sceneAssets[i];
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!files.Contains(scenePath))
                {
                    files.Add(scenePath);
                }
            }
            
            //所有shader的集合(单独一个包)
            HashSet<string> shaders = new HashSet<string>();
            //需要移除的文件
            List<string> needRemoveFile = new List<string>();
            //分解所有需要加载的文件
            foreach (string file in files)
            {
                //判断是否是 Shader 文件
                if (BuildAssetTool.IsShaderAsset(file))
                {
                    Shader shaderObj = AssetDatabase.LoadAssetAtPath<Shader>(file);
                    if (shaderObj != null && alwaysIncludedShaders.Contains(shaderObj.name))
                    {
                        continue;
                    }
                    if (!shaders.Contains(file))
                    {
                        shaders.Add(file);
                    }
                    //Shader 文件需要移除，不直接进行加载
                    if (!needRemoveFile.Contains(file))
                    {
                        needRemoveFile.Add(file);
                    }
                    continue;
                }
                //判断是否是组文件
                string fileGroupPath = null;
                foreach (string assetGroupPath in subBundle.AssetGroupPaths)
                {
                    //是组文件，则移除该文件，由组进行打成 AB 包
                    if (BuildAssetTool.IsGroupFile(file, assetGroupPath))
                    {
                        fileGroupPath = assetGroupPath;
                        needRemoveFile.Remove(file);
                        break;
                    }
                }
                //不是组文件则需要单独打成 AB 包
                if (fileGroupPath == null)
                {
                    ResFile res = new ResFile(file, $"{GetBundleName(subBundle, file)}.{subBundle.BundleVariant}");
                    resFiles.Add(file, res);
                }
                
                //获取依赖
                string[] depends = AssetDatabase.GetDependencies(file);
                //过滤出真正需要加载的依赖
                List<string> realDepends = new List<string>();
                //分析依赖情况
                for (int i = 0; i < depends.Length; i++)
                {
                    string depend = depends[i];
                    //不可以依赖自身
                    if (depend == file)
                    {
                        continue;
                    }
                    //不属于资源文件
                    if (!depend.StartsWith("Assets/"))
                    {
                        continue;
                    }
                    //脚本不能作为依赖
                    if (depend.EndsWith(".cs"))
                    {
                        continue;
                    }
                    //shader 不作为依赖，shader 单独作为包体
                    if (BuildAssetTool.IsShaderAsset(depend))
                    {
                        Shader shaderObj = AssetDatabase.LoadAssetAtPath<Shader>(depend);
                        if (shaderObj != null && alwaysIncludedShaders.Contains(shaderObj.name))
                        {
                            continue;
                        }
                        if (!shaders.Contains(depend))
                        {
                            shaders.Add(depend);
                        }
                        continue;
                    }
                    //无法被加载的文件，不作为依赖
                    if (BuildAssetTool.CantLoadFile(depend))
                    {
                        continue;
                    }
                    //判断是否是组文件
                    string dependGroupPath = null;
                    foreach (string assetGroupPath in subBundle.AssetGroupPaths)
                    {
                        //是组文件
                        if (BuildAssetTool.IsGroupFile(depend, assetGroupPath))
                        {
                            dependGroupPath = assetGroupPath;
                            //不同组，需要添加依赖
                            if (assetGroupPath != fileGroupPath && !realDepends.Contains(assetGroupPath))
                            {
                                if (!realDepends.Contains(assetGroupPath))
                                {
                                    realDepends.Add(assetGroupPath);
                                }
                            }
                            break;
                        }
                    }
                    //不是组文件, 添加对应依赖
                    if (dependGroupPath == null)
                    {
                        //作为依赖计算被依赖的次数
                        if (dependenciesIndex.ContainsKey(depend))
                        {
                            dependenciesIndex[depend]++;
                        }
                        else
                        {
                            dependenciesIndex.Add(depend, 1);
                        }
                        
                        //作为文件被依赖了直接添加就行
                        realDepends.Add(depend);
                    }
                }
                
                //添加依赖
                if (fileGroupPath != null)
                {
                    resGroups[fileGroupPath].FilePathList.Add(file);
                    groupFileToDepends.Add(file, realDepends);
                }
                else
                {
                    fileDepends.Add(file, realDepends.ToArray());
                }
            }
            //移除不需要的文件
            foreach (string removeFile in needRemoveFile)
            {
                files.Remove(removeFile);
            }
            
            //分析组包
            foreach (ResGroup resGroup in resGroups.Values)
            {
                foreach (string groupFile in resGroup.FilePathList)
                {
                    foreach (string realDepend in groupFileToDepends[groupFile])
                    {
                        //该依赖是否包含在文件中
                        if (resGroup.FilePathList.Contains(realDepend))
                        {
                            continue;
                        }

                        if (!dependenciesIndex.ContainsKey(realDepend))
                        {
                            string realDependGroup = null;
                            foreach (var assetGroupPath in subBundle.AssetGroupPaths)
                            {
                                //是否是组文件
                                if (BuildAssetTool.IsGroupFile(realDepend, assetGroupPath))
                                {
                                    realDependGroup = assetGroupPath;
                                    if (!resGroup.DependFileName.Contains(realDependGroup))
                                    {
                                        resGroup.DependFileName.Add(realDependGroup);
                                    }

                                    break;
                                }
                            }
                            if (realDependGroup != null)
                            {
                                continue;
                            }
                        }
                        //如果依赖只依赖与他，则退出，会自动打包进来
                        if (dependenciesIndex[realDepend] == 1)
                        {
                            continue;
                        }
                        else
                        {
                            //说明这个依赖是单独依赖并且被Group依赖
                            if (!resDepends.ContainsKey(realDepend))
                            {
                                ResDepend resDepend = new ResDepend(realDepend, 
                                    $"{GetBundleName(subBundle, realDepend)}.{subBundle.BundleVariant}");
                                resDepends.Add(realDepend, resDepend);
                            }
                        }
                        //组包添加该依赖包
                        if (!resGroup.DependFileName.Contains(realDepend))
                        {
                            resGroup.DependFileName.Add(realDepend);
                        }
                    }
                }
            }
            
            //被多次依赖的文件
            HashSet<string> compoundDepends = new HashSet<string>();
            foreach (var dependFile in dependenciesIndex)
            {
                if (dependFile.Value > 1)
                {
                    compoundDepends.Add(dependFile.Key);
                }
            }
            //添加依赖信息
            foreach (var fileDepend in fileDepends)
            {
                ResFile resFile = resFiles[fileDepend.Key];
                List<string> depends = new List<string>();
                foreach (string depend in fileDepend.Value)
                {
                    if (compoundDepends.Contains(depend))
                    {
                        //说明这个被依赖项是一个单独的bundle
                        depends.Add(depend);
                        //被依赖项也要创建Res类
                        if (!resDepends.ContainsKey(depend))
                        {
                            ResDepend resDepend = new ResDepend(depend, 
                                $"{GetBundleName(subBundle, depend)}.{subBundle.BundleVariant}");
                            resDepends.Add(depend, resDepend);
                        }
                    }
                    else
                    {
                        //依赖是文件
                        if (files.Contains(depend))
                        {
                            depends.Add(depend);
                        }
                        //依赖是组资源
                        foreach (string groupPath in subBundle.AssetGroupPaths)
                        {
                            if (BuildAssetTool.IsGroupFile(depend, groupPath))
                            {
                                if (!depends.Contains(groupPath))
                                {
                                    depends.Add(groupPath);
                                }

                                break;
                            }
                        }
                    }
                }
                
                resFile.DependFileName = depends;
            }
            
            //创建需要的Bundle包
            List<AssetBundleBuild> allAssetBundleBuild = new List<AssetBundleBuild>();
            
            //首先创建Shader
            AssetBundleBuild shaderBundle = new AssetBundleBuild();
            shaderBundle.assetBundleName = "shader_" + subBundle.BuildName;
            shaderBundle.assetNames = shaders.ToArray();
            allAssetBundleBuild.Add(shaderBundle);
            
            //添加文件包
            AddToAssetBundleBuilds(subBundle, allAssetBundleBuild, files);
            
            //添加依赖包
            AddToAssetBundleBuilds(subBundle, allAssetBundleBuild, compoundDepends);
            
            //添加组包
            foreach (ResGroup resGroup in resGroups.Values)
            {
                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = resGroup.AssetBundleName;
                assetBundleBuild.assetNames = resGroup.FilePathList.ToArray();
                allAssetBundleBuild.Add(assetBundleBuild);
            }
            
            Debug.Log("文件包数量：" + files.Count);
            Debug.Log("依赖包数量：" + compoundDepends.Count);
            Debug.Log("组包数量：" + resGroups.Count);
            
            //没有包体资源
            if (allAssetBundleBuild.Count <= 1)
            {
                Debug.LogError("没有资源: " + subBundle.BuildName);
                return;
            }

            Debug.Log("AB 包数量：" + allAssetBundleBuild.Count);

            //保存Log
            SaveLoadLog(bundleTable, subBundle, resFiles, resDepends, resGroups);
            
            //开始打包
            string bundlePackagePath = Path.Combine(bundleTable.BuildBundlePath, subBundle.BuildName);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(bundlePackagePath, allAssetBundleBuild.ToArray(), 
                subBundle.BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);
            
            //保存版本号资源文件
            SaveBundleVersionFile(bundlePackagePath, manifest, subBundle);
            
            //存储记录所有资源的可加载路径
            foreach (string assetPath in resFiles.Keys)
            {
                if (!assetLoadPath.Contains(assetPath))
                {
                    assetLoadPath.Add(assetPath);
                }
            }
            foreach (ResGroup resGroup in resGroups.Values)
            {
                foreach (string filePathList in resGroup.FilePathList)
                {
                    if (!assetLoadPath.Contains(filePathList))
                    {
                        assetLoadPath.Add(filePathList);
                    }
                }
            }
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
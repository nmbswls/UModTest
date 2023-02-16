using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [MenuItem("Tool/GenerateAssetBundle")]
    private static void BuildAsset()
    {
        string resPath = "Assets/Res";
        string abOutputPath = "AssetBundles/";
        var bundleFolderList = Directory.GetDirectories(resPath,
                "*",
                SearchOption.TopDirectoryOnly);

        List<AssetBundleBuild> buildInfoList = new List<AssetBundleBuild>();

        foreach (var bundleFolderName in bundleFolderList)
        {
            // 遍历每一个文件，非.meta结尾的文件设置bundle name
            string[] fileNames = Directory.GetFiles(bundleFolderName, "*", SearchOption.AllDirectories);
            var assetList = new List<string>();
            foreach (string fileName in fileNames)
            {
                // .meta结尾,(DS_Store mac 下的系统文件),跳过
                if (fileName.EndsWith(".meta") || fileName.EndsWith("DS_Store"))
                {
                    continue;
                }

                assetList.Add(ReformPathString(fileName));
            }
            DirectoryInfo d = new DirectoryInfo(bundleFolderName);
            
            if (assetList.Count > 0)
            {
                AssetBundleBuild pBuild = new AssetBundleBuild();
                pBuild.assetBundleName = d.Name;
                pBuild.assetNames = assetList.ToArray();
                buildInfoList.Add(pBuild);
            }
        }

        if (!Directory.Exists(abOutputPath))
        {
            Directory.CreateDirectory(abOutputPath);
        }

        // 刷新编辑器
        AssetDatabase.Refresh();

        var mManifest = BuildPipeline.BuildAssetBundles(abOutputPath, buildInfoList.ToArray(),
                BuildAssetBundleOptions.StrictMode |  BuildAssetBundleOptions.DisableWriteTypeTree,
                EditorUserBuildSettings.activeBuildTarget);

        if (mManifest == null || mManifest.GetAllAssetBundles() == null || mManifest.GetAllAssetBundles().Length == 0)
        {
            UnityEngine.Debug.LogError("BuildPipeline.BuildAssetBundlesConfigData() Error");
            return;
        }

        // 刷新编辑器
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 规范化路径字符串
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReformPathString(string path)
    {
        path = path.Replace("\\", "/");
        path = path.Replace("//", "/");

        while (path.EndsWith("/"))
            path = path.Substring(0, path.Length - 1);
        return path;
    }
}

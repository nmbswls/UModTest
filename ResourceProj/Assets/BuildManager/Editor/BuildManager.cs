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
            // ����ÿһ���ļ�����.meta��β���ļ�����bundle name
            string[] fileNames = Directory.GetFiles(bundleFolderName, "*", SearchOption.AllDirectories);
            var assetList = new List<string>();
            foreach (string fileName in fileNames)
            {
                // .meta��β,(DS_Store mac �µ�ϵͳ�ļ�),����
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

        // ˢ�±༭��
        AssetDatabase.Refresh();

        var mManifest = BuildPipeline.BuildAssetBundles(abOutputPath, buildInfoList.ToArray(),
                BuildAssetBundleOptions.StrictMode |  BuildAssetBundleOptions.DisableWriteTypeTree,
                EditorUserBuildSettings.activeBuildTarget);

        if (mManifest == null || mManifest.GetAllAssetBundles() == null || mManifest.GetAllAssetBundles().Length == 0)
        {
            UnityEngine.Debug.LogError("BuildPipeline.BuildAssetBundlesConfigData() Error");
            return;
        }

        // ˢ�±༭��
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// �淶��·���ַ���
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

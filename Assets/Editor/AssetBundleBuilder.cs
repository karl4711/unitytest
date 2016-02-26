using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class AssetBundleBuilder : MonoBehaviour
{
    public static string sourcePath = Application.dataPath + "/Resources";
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";

    static int version = 0;

    private static Dictionary<string, string> localFileDict = new Dictionary<string, string>();

    [MenuItem("AssetBundleBuilder/Build Bundles/PC", false, 1)]
    public static void BuildPCBundles()
    {
        BuildBundles(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("AssetBundleBuilder/Build Bundles/Android", false, 1)]
    public static void BuildAndroidBundles()
    {
        BuildBundles(BuildTarget.Android);
    }

    [MenuItem("AssetBundleBuilder/Build Bundles/iOS", false, 1)]
    public static void BuildIOSBundles()
    {
        BuildBundles(BuildTarget.iOS);
    }

    [MenuItem("AssetBundleBuilder/Init Bundle Names", false, 2)]
    public static void InitNames()
    {
        ClearAssetBundlesName();
        AddMultiBundleNames(sourcePath);
    }



    static void BuildBundles(BuildTarget target)
    {
        string outputPath = Path.Combine(AssetBundlesOutputPath, GetPlatformFolder(target)); //EditorUserBuildSettings.activeBuildTarget
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        else // clear bundle folder
        {
            try
            {
                string currentVersion = File.ReadAllText(CommonUtil.Instance.LocalVersionPath(GetPlatformFolder(target)));
                version = Convert.ToInt32(currentVersion) + 1;
            }
            catch (Exception e)
            {
                Debug.Log("Auto modify version failed.\n" + e);
            }

            DirectoryInfo di = new DirectoryInfo(outputPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        //build asset bundles
        BuildPipeline.BuildAssetBundles(outputPath,  //BuildAssetBundleOptions.ForceRebuildAssetBundle |
            BuildAssetBundleOptions.AppendHashToAssetBundleName, target);

        //create filedict and version
        CreateLocalFileDict(target);
        File.WriteAllText(CommonUtil.Instance.LocalVersionPath(GetPlatformFolder(target)), version.ToString());

        AssetDatabase.Refresh();

        Debug.Log("build asset bundles complete.");

    }

    /// <summary>
    /// 构造本地文件Dict(filename:hash)
    /// </summary>
    /// <param name="path"></param>
    //[MenuItem("AssetBundleBuilder/Create Local File Dict", false, 3)]
    static void CreateLocalFileDict(BuildTarget target)
    {
        localFileDict.Clear();
        CreateLocalFileDict(Path.Combine(Application.streamingAssetsPath, CommonUtil.Instance.GetRuntimePlatform()));
        //Debug.Log("CommonUtil.localFileDictPath: " + CommonUtil.Instance.LocalFileDictPath(GetPlatformFolder(target)));

        using (StreamWriter writetext = new StreamWriter(CommonUtil.Instance.LocalFileDictPath(GetPlatformFolder(target))))
        {
            localFileDict.ToList().ForEach(kv => writetext.WriteLine(string.Format("{0}|{1}", kv.Key, kv.Value)));
        }

        //StringBuilder sBuilder = new StringBuilder();
        //localFileDict.ToList().ForEach(kv => sBuilder.Append(string.Format("{0}|{1}\n", kv.Key, kv.Value)));
        //File.WriteAllText(CommonUtil.localFileDictPath, sBuilder.ToString());

        AssetDatabase.Refresh();
    }

    static void CreateLocalFileDict(string path)
    {
        List<string> localFileList = new List<string>();
        DirectoryInfo folder = new DirectoryInfo(path);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                CreateLocalFileDict(files[i].FullName);
            }
            else if (files[i].Name.EndsWith(".unity3d"))
            {
                localFileList.Add(files[i].FullName);
            }
        }

        for (int i = 0; i < localFileList.Count; i++)
        {
            string _source = CommonUtil.Instance.ReplaceSlash(localFileList[i]);
            localFileList[i] = _source.Substring(Application.streamingAssetsPath.Length + CommonUtil.Instance.GetRuntimePlatform().Length + 2);

            //Debug.Log(localFileList[i]);
            string key = localFileList[i].Substring(0, localFileList[i].Length - 41);
            string value = localFileList[i].Substring(localFileList[i].Length - 40, 32);

            //Debug.Log("key: " + key + ", value: " + value);
            localFileDict.Add(key, value);
        }
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下
    /// </summary>
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
    }

    /// <summary>
    /// 批量添加指定目录下的AssetBundleName;
    /// 当前方法为最大拆分力度，即对每个资源单独添加AssetBundleName.
    /// </summary>
    /// <param name="source"></param>
    static void AddMultiBundleNames(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                AddMultiBundleNames(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    AddBundleNameToFile(files[i].FullName);
                }
            }
        }
    }

    static void AddBundleNameToFile(string source)
    {
        string _source = CommonUtil.Instance.ReplaceSlash(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
        //Debug.Log (_assetPath);

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        assetName = assetName.Replace(Path.GetExtension(assetName), ".unity3d");
        //Debug.Log (assetName);
        assetImporter.assetBundleName = assetName;
    }



    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            case BuildTarget.WebPlayer:
                return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
            default:
                return null;
        }
    }
}




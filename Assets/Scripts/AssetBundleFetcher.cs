using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// 服务器资源获取类
/// </summary>
public class AssetBundleFetcher : MonoBehaviour
{
    private string platform;

    private string serverAssetPath;
    private string serverFileDictPath;
    private string serverVersionPath;
    private string serverFileDictText;

    private string localAssetPath;
    private string localFileDictPath;
    private string localVersionPath;

    public static Dictionary<string, string> localFileDict = new Dictionary<string, string>();
    private static Dictionary<string, string> serverFileDict = new Dictionary<string, string>();

    private static string localVersion;
    private static string serverVersion;

    private static List<string> fetchFileList = new List<string>();
    private static List<string> removeFileList = new List<string>();

    void Awake()
    {
        platform = CommonUtil.Instance.GetRuntimePlatform();

        //本地测试目录
        serverAssetPath = "file://" + Application.streamingAssetsPath + "/Server/" + platform + "/";   //  including platform, endwith '/'
        serverVersionPath = serverAssetPath + "version.txt";
        serverFileDictPath = serverAssetPath + "fileDict.txt";

        localAssetPath = Path.Combine(Application.streamingAssetsPath, platform);
        localVersionPath = CommonUtil.Instance.LocalVersionPath(platform);
        localFileDictPath = CommonUtil.Instance.LocalFileDictPath(platform);

    }

    /// <summary>
    /// 4. 下载文件
    /// </summary>
    public void FetchFiles(Action callback)
    {
        Action<List<WWW>> fetchWWWsFinishedAction = (wwws) =>
        {
            wwws.ForEach(www =>
            {
                CommonUtil.Instance.ReplaceLocalFile(Path.Combine(localAssetPath, www.url.Replace(serverAssetPath, "")), www.bytes);
            });

            //覆盖filedict, version.
            File.WriteAllText(localVersionPath, serverVersion);
            File.WriteAllText(localFileDictPath, serverFileDictText);

            //删除本地文件
            removeFileList.ForEach(f =>
                        {
                            try
                            {
                                File.Delete(Path.Combine(localAssetPath, f));
                                File.Delete(Path.Combine(localAssetPath, f + ".meta")); //need do this?
                            }
                            catch (Exception e) {
                                Debug.Log("delete file failed.\n" + e);
                            }
                        }
                    );

            callback();
        };

        Action fetchFileListFinishedAction = delegate
        {
            List<string> fetchPathList = new List<string>();
            fetchFileList.ForEach(f => fetchPathList.Add(serverAssetPath + f));

            //fetchPathList.ForEach(p => Debug.Log("fetchPathList: " + p));

            StartCoroutine(CommonUtil.Instance.FetchMultRes(fetchPathList, fetchWWWsFinishedAction));
        };

        CompareFileDicts(fetchFileListFinishedAction);
    }


    /// <summary>
    /// 3. 比较本地和服务器文件Dict, 获取需要下载的文件列表
    /// </summary>
    /// <param name="callback"></param>
    void CompareFileDicts(Action callback)
    {
        Action fetchServerFileDictFinishedAction = delegate
        {
            // 添加本地没有的文件到fetch list
            serverFileDict.Where(
                        kv => !localFileDict.ContainsKey(kv.Key))
                        .ToList().ForEach(
                            kv => fetchFileList.Add(string.Format("{0}_{1}.unity3d", kv.Key, kv.Value))
                    );
            // 添加有更新的文件到fetch list, 本地文件到remove list
            serverFileDict.Where(
                        kv => localFileDict[kv.Key] != kv.Value)
                        .ToList().ForEach(
                            kv =>
                            {
                                fetchFileList.Add(string.Format("{0}_{1}.unity3d", kv.Key, kv.Value));
                                removeFileList.Add(string.Format("{0}_{1}.unity3d", kv.Key, localFileDict[kv.Key]));
                            });

            //添加manifest文件到fetch list
            fetchFileList.Add(platform);

            //fetchFileList.ForEach(f => Debug.Log("fetchFileList: " + f));
            //removeFileList.ForEach(f => Debug.Log("removeFileList: " + f));

            callback();
        };

        Action loadLocalFileDictFinishedAction = delegate
        {
            FetchServerFileDict(fetchServerFileDictFinishedAction);
        };

        LoadLocalFileDict(loadLocalFileDictFinishedAction);
    }


    /// <summary>
    /// 2. 获取服务器文件Dict
    /// </summary>
    /// <param name="callback"></param>
    void FetchServerFileDict(Action callback)
    {
        Action<string> loadServerFileFinishedAction = (text) =>
        {
            serverFileDictText = text;
            string[] strs = text.Split('\r', '\n');
            strs.ToList().ForEach(a =>
            {
                if (a.Trim() != "")
                {
                    string[] kv = a.Split('|');
                    serverFileDict.Add(kv[0], kv[1]);
                }
            });
            callback();
        };

        StartCoroutine(FetchTextFileCoroutine(serverFileDictPath, loadServerFileFinishedAction));
    }


    /// <summary>
    /// 1. 获取本地文件Dict
    /// </summary>
    /// <param name="callback"></param>
    public void LoadLocalFileDict(Action callback)
    {
        Action<string> loadLocalFileFinishedAction = (text) =>
        {
            string[] strs = text.Split('\r', '\n');
            strs.ToList().ForEach(a =>
            {
                if (a.Trim() != "")
                {
                    string[] kv = a.Split('|');
                    localFileDict.Add(kv[0], kv[1]);
                }
            });
            callback();
        };

        StartCoroutine(FetchTextFileCoroutine("file://" + localFileDictPath, loadLocalFileFinishedAction));
    }

    /// <summary>
    /// 0. 检查是否需要更新版本
    /// </summary>
    /// <param name="callback"></param>
    public void CheckIfVersionChanged(Action<bool> callback)
    {
        //Debug.Log("localVersionPath: " + localVersionPath);
        //Debug.Log("serverVersionPath: " + serverVersionPath);

        Action<string> loadServerVersionFinishedAction = (version) =>
        {
            serverVersion = version;
            Debug.Log("serverVersion: " + serverVersion);
            callback(localVersion != serverVersion);
        };

        localVersion = File.ReadAllText(localVersionPath);
        Debug.Log("localVersion: " + localVersion);
        StartCoroutine(FetchTextFileCoroutine(serverVersionPath, loadServerVersionFinishedAction));
    }


    #region Util
    /// <summary>
    /// 拉取文本资源
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator FetchTextFileCoroutine(string filePath, Action<string> callback)
    {
        WWW file = new WWW(filePath);
        yield return file;
        callback(file.text);
        file.Dispose();
    }


    #endregion
}

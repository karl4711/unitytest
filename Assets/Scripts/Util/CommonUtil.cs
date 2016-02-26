using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

public class CommonUtil
{
    private static CommonUtil commonUtil;

    public static CommonUtil Instance
    {
        get
        {
            if (commonUtil == null)
            {
                commonUtil = new CommonUtil();
            }
            return commonUtil;
        }
    }

    /// <summary>
    /// 本地fileDict文件路径
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public string LocalFileDictPath(string platform)
    {
        return Path.Combine(Path.Combine(Application.streamingAssetsPath, platform), "fileDict.txt").Replace("\\", "/");
    }

    /// <summary>
    /// 本地version文件路径
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public string LocalVersionPath(string platform)
    {
        return Path.Combine(Path.Combine(Application.streamingAssetsPath, platform), "version.txt").Replace("\\", "/");
    }

    /// <summary>
    /// 统一分隔符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string ReplaceSlash(string s)
    {
        return s.Replace("\\", "/");
    }

    /// <summary>
    /// 获取运行平台对应文件夹名称
    /// </summary>
    /// <returns></returns>
    public string GetRuntimePlatform()
    {
        string platform = "";
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            platform = "Windows";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            platform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            platform = "IOS";
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            platform = "OSX";
        }
        return platform;
    }

    /// <summary>
    /// 更新本地文件;
    /// </summary>
    /// <param name="url"></param>
    /// <param name="res"></param>
    public void ReplaceLocalFile(string url, byte[] res)
    {
        //Debug.Log(url);
        //Debug.Log(res.Length);
        FileStream file = new FileStream(url, FileMode.OpenOrCreate);
        file.Write(res, 0, res.Length);
        file.Flush();
        file.Close();
    }

    /// <summary>
    /// 加载多个资源;
    /// </summary>
    /// <param name="paths"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator FetchMultRes(List<string> paths, Action<List<WWW>> callback)
    {
        List<WWW> wwws = new List<WWW>();

        for (int i = 0; i < paths.Count; i++)
        {
            //Debug.Log("LoadRes: " + paths[i]);

            wwws.Add(new WWW(paths[i]));
            yield return wwws[i];
        }
        callback(wwws);
        wwws.ForEach(w => w.Dispose());
    }


}

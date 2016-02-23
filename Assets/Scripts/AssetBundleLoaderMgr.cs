using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
 
public class AssetBundleLoaderMgr : MonoBehaviour
{
    public string m_assetPath;
    string assetTail = ".unity3d";

//    #region Instance
//    private static AssetBundleLoaderMgr instance;
//
//    public static AssetBundleLoaderMgr Instance()
//    {
//        if (instance == null)
//        {
//            instance = new AssetBundleLoaderMgr();
//        }
//        return instance;
//    }
//    #endregion

	void Awake()
	{
		m_assetPath = Application.streamingAssetsPath;
	}

    #region LoadAssetBundle

    /// <summary>
    /// 加载目标资源
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    public void LoadAssetBundle(string name, Action<UnityEngine.Object> callback)
    {
        name = name + assetTail;//eg:ui/panel.unity3d
 
        Action<List<AssetBundle>> action = (depenceAssetBundles) =>
        {
 
            string realName = this.GetRuntimePlatform() + "/" + name;//eg:Windows/ui/panel.unity3d
 
            LoadResReturnWWW(realName, (www) =>
            {
                int index = realName.LastIndexOf("/");
                string assetName = realName.Substring(index + 1);
                assetName = assetName.Replace(assetTail, "");
                AssetBundle assetBundle = www.assetBundle;
                UnityEngine.Object obj = assetBundle.LoadAsset(assetName);//LoadAsset(name）,这个name没有后缀,eg:panel
 
                //卸载资源内存
                assetBundle.Unload(false);
                for (int i = 0; i < depenceAssetBundles.Count; i++)
                {
                    depenceAssetBundles[i].Unload(false);
                }
 
                //加载目标资源完成的回调
                callback(obj);
            });
 
        };
 
        LoadDependenceAssets(name, action);
    }
 
    /// <summary>
    /// 加载目标资源的依赖资源
    /// </summary>
    /// <param name="targetAssetName"></param>
    /// <param name="action"></param>
    private void LoadDependenceAssets(string targetAssetName, Action<List<AssetBundle>> action)
    {
        Debug.Log("要加载的目标资源:" + targetAssetName);//ui/panel.unity3d
        Action<AssetBundleManifest> dependenceAction = (manifest) =>
        {
            List<AssetBundle> depenceAssetBundles = new List<AssetBundle>();//用来存放加载出来的依赖资源的AssetBundle
 
            string[] dependences = manifest.GetAllDependencies(targetAssetName);
            Debug.Log("依赖文件个数：" + dependences.Length);
            int length = dependences.Length;
            int finishedCount = 0;
            if (length == 0)
            {
                //没有依赖
                action(depenceAssetBundles);
            }
            else
            {
                //有依赖，加载所有依赖资源
                for (int i = 0; i < length; i++)
                {
                    string dependenceAssetName = dependences[i];
                    dependenceAssetName = GetRuntimePlatform() + "/" + dependenceAssetName;//eg:Windows/altas/heroiconatlas.unity3d
 
                    //加载，加到assetpool
                    LoadResReturnWWW(dependenceAssetName, (www) =>
                    {
                        int index = dependenceAssetName.LastIndexOf("/");
                        string assetName = dependenceAssetName.Substring(index + 1);
                        assetName = assetName.Replace(assetTail, "");
                        AssetBundle assetBundle = www.assetBundle;
                        UnityEngine.Object obj = assetBundle.LoadAsset(assetName);
                        //assetBundle.Unload(false);
                        depenceAssetBundles.Add(assetBundle);
 
                        finishedCount++;
 
                        if (finishedCount == length)
                        {
                            //依赖都加载完了
                            action(depenceAssetBundles);
                        }
                    });
                }
            }
        };
        LoadAssetBundleManifest(dependenceAction);
    }
 
    /// <summary>
    /// 加载AssetBundleManifest
    /// </summary>
    /// <param name="action"></param>
    private void LoadAssetBundleManifest(Action<AssetBundleManifest> action)
    {
        string manifestName = this.GetRuntimePlatform();
        manifestName = manifestName + "/" + manifestName;//eg:Windows/Windows
        Debug.Log("manifestName: " + manifestName);

        LoadResReturnWWW(manifestName, (www) =>
        {
            AssetBundle assetBundle = www.assetBundle;
            UnityEngine.Object obj = assetBundle.LoadAsset("AssetBundleManifest");
            assetBundle.Unload(false);
            AssetBundleManifest manif = obj as AssetBundleManifest;
            action(manif);
        });
    }
    #endregion
 
    #region ExcuteLoader
    public void LoadResReturnWWW(string name, Action<WWW> callback)
    {
        string path = "file://" + this.m_assetPath + "/" + name;
        Debug.Log("加载：" + path);
        StartCoroutine(LoaderRes(path, callback));
    }
 
    IEnumerator LoaderRes(string path, Action<WWW> callback)
    {
        WWW www = new WWW(path);
        yield return www;
        
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);
		}

		if (www.isDone)
        {
            callback(www);
        }
    }
    #endregion
 
    #region Util
    /// <summary>
    /// 平台对应文件夹
    /// </summary>
    /// <returns></returns>
    private string GetRuntimePlatform()
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
}
#endregion
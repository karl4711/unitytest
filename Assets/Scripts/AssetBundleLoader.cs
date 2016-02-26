using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


/// <summary>
/// 本地资源加载类
/// </summary>
public class AssetBundleLoader : MonoBehaviour
{
    public string m_assetPath;
    public static string assetTail = ".unity3d";

    void Awake()
    {
        m_assetPath = "file://" + Application.streamingAssetsPath + "/";
    }

    #region LoadAssetBundle

    /// <summary>
    /// 加载目标资源
    /// </summary>
    /// <param name="names"></param>
    /// <param name="callback"></param>
    public void LoadAssetBundles(List<string> names, Action<UnityEngine.Object[]> callback)
    {
        //		StartCoroutine (LoadBundleCoroutine(names, callback));

        Action<List<AssetBundle>> action = (depenceAssetBundles) =>
        {
            List<string> realNames = new List<string>();
            names.ForEach(name => realNames.Add(getRealLocalNameWithPlatForm(name)));//eg:Windows/ui/panel.unity3d

            FetchResReturnWWW(realNames, wwws =>
            {
                List<UnityEngine.Object> objList = new List<UnityEngine.Object>();
                foreach (WWW www in wwws)
                {
                    Debug.Log(www.url);
                    AssetBundle assetBundle = www.assetBundle;
                    UnityEngine.Object[] objs = assetBundle.LoadAllAssets<UnityEngine.Object>();//LoadAsset(name）,这个name没有后缀,eg:panel

                    objList = objList.Concat(objs).ToList();

                    assetBundle.Unload(false); //卸载资源内存
                }

                for (int i = 0; i < depenceAssetBundles.Count; i++)
                {
                    depenceAssetBundles[i].Unload(false);
                }

                //加载目标资源完成的回调
                callback(objList.ToArray());
            });

        };

        LoadDependenceAssets(names, action);
    }

    /// <summary>
    /// 加载目标资源的依赖资源
    /// </summary>
    /// <param name="targetAssetNames"></param>
    /// <param name="action"></param>
    private void LoadDependenceAssets(List<string> targetAssetNames, Action<List<AssetBundle>> action)
    {
        targetAssetNames.ForEach(n => Debug.Log("targetAssetName:" + n));//ui/panel.unity3d

        Action<AssetBundleManifest> dependenceAction = (manifest) =>
        {


            List<AssetBundle> depenceAssetBundles = new List<AssetBundle>();//用来存放加载出来的依赖资源的AssetBundle

            List<string> dependencyNames = new List<string>();

            foreach (string targetAssetName in targetAssetNames)
            {
                //string[] dependencies = manifest.GetAllDependencies(targetAssetName);
                //Debug.Log("dependencies length: " + dependencies.Length);

                dependencyNames = dependencyNames.Union(manifest.GetAllDependencies(getRealLocalName(targetAssetName))).ToList();
            }
            int length = dependencyNames.Count;
            //Debug.Log("dependency length: " + length);
            if (length == 0)
            {
                //没有依赖
                action(depenceAssetBundles);
            }
            else {
                //有依赖，加载所有依赖资源
                for (int i = 0; i < length; i++)
                {
                    dependencyNames[i] = CommonUtil.Instance.GetRuntimePlatform() + "/" + dependencyNames[i];//eg:Windows/altas/heroiconatlas.unity3d
                    //Debug.Log("dependencyNames: " + dependencyNames[i]);
                }
                //加载，加到assetpool
                FetchResReturnWWW(dependencyNames, (wwws) =>
                {
                    wwws.ForEach(w => depenceAssetBundles.Add(w.assetBundle));
                    action(depenceAssetBundles);
                });
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
        string manifestName = CommonUtil.Instance.GetRuntimePlatform();
        manifestName = manifestName + "/" + manifestName;//eg:Windows/Windows
        Debug.Log("manifestName: " + manifestName);

        FetchResReturnWWW(new List<string> { manifestName }, (www) =>
        {
            AssetBundle assetBundle = www[0].assetBundle;
            UnityEngine.Object obj = assetBundle.LoadAsset("AssetBundleManifest");
            assetBundle.Unload(false);
            AssetBundleManifest manif = obj as AssetBundleManifest;
            //Debug.Log("manif: " + manif);
            action(manif);
        });
    }

    #endregion

    #region Util

    void FetchResReturnWWW(List<string> names, Action<List<WWW>> callback)
    {
        List<string> pathList = new List<string>();
        names.ForEach(n => pathList.Add(m_assetPath + n));
        StartCoroutine(CommonUtil.Instance.FetchMultRes(pathList, callback));
    }

    string getRealLocalNameWithPlatForm(string originName)
    {
        return string.Format("{0}/{1}_{2}{3}",
                    CommonUtil.Instance.GetRuntimePlatform(), originName,
                    AssetBundleFetcher.localFileDict[originName], assetTail);
    }

    string getRealLocalName(string originName)
    {
        return string.Format("{0}_{1}{2}", originName,
                    AssetBundleFetcher.localFileDict[originName], assetTail);
    }

    #endregion
}

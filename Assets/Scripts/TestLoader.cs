using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TestLoader : MonoBehaviour
{
    public List<PrefabAttr> prefabList;

    Dictionary<string, GameObject> GameObjectPool = new Dictionary<string, GameObject>();

    AssetBundleLoader assetBundleLoader;

    AssetBundleFetcher assetBundleFetcher;

    void Awake()
    {
        assetBundleLoader = GameObject.Find("AssetBundleLoader").GetComponent<AssetBundleLoader>();
        assetBundleFetcher = GameObject.Find("AssetBundleFetcher").GetComponent<AssetBundleFetcher>();
    }

    void Start()
    {
        DownloadRes(delegate
       {
           prefabList = new List<PrefabAttr>()
           {
                new PrefabAttr("1", new Vector3(-100, -100, 0),true),
                new PrefabAttr("2", new Vector3(100, 100, 0), true)
           };
           LoadLocalRes(prefabList);
       });
    }

    /// <summary>
    /// 下载服务器资源
    /// </summary>
    /// <param name="callback"></param>
    void DownloadRes(Action callback)
    {
        assetBundleFetcher.CheckIfVersionChanged(isChanged =>
        {
            if (isChanged)
            {
                assetBundleFetcher.FetchFiles(callback);
            }
            else
            {
                Debug.Log("version not change.");

                //need this to load local bundles.
                assetBundleFetcher.LoadLocalFileDict(callback);
            }

        });
    }

    /// <summary>
    /// 加载本地资源
    /// </summary>
    /// <param name="prefabList"></param>
    void LoadLocalRes(List<PrefabAttr> prefabList)
    {
        List<string> assetNameList = new List<string>();
        prefabList.ForEach(prefab => assetNameList.Add("prefabs/" + prefab.Name));

        assetBundleLoader.LoadAssetBundles(assetNameList, (objs) =>
        {
            objs.ToList().ForEach(o =>
                   {
                       PrefabAttr attr = prefabList.Find(a => a.Name == o.name);

                       //Debug.Log("o.name:" + o.name);
                       GameObject go = GameObject.Instantiate(o, attr.Position, Quaternion.identity) as GameObject;
                       go.SetActive(attr.IsActive);
                       //加载出来的GameObject放到GameObjectPool存储
                       GameObjectPool.Add(o.name, go);
                   });

        });

    }

}

/// <summary>
/// 场景中prefab属性类
/// </summary>
public class PrefabAttr
{
    /// <summary>
    /// 名称
    /// </summary>
    private string name;

    /// <summary>
    /// 位置
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// 是否active
    /// </summary>
    private bool isActive;

    public PrefabAttr(string name)
    {
        Name = name;
        position = Vector3.zero;
        isActive = true;
    }

    /// <summary>
    /// prefab属性
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="position">位置</param>
    /// <param name="isActive">是否active</param>
    public PrefabAttr(string name, Vector3 position, bool isActive)
    {
        Name = name;
        Position = position;
        IsActive = isActive;
    }


    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
        }
    }
}
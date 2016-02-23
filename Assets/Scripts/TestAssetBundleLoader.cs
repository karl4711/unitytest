using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class TestAssetBundleLoader : MonoBehaviour
{
    Dictionary<string, GameObject> GameObjectPool = new Dictionary<string, GameObject>();
 	
	AssetBundleLoaderMgr assetBundleLoaderMgr;

	void Awake()
	{
		assetBundleLoaderMgr = GameObject.Find ("AssetBundleLoaderMgr").GetComponent<AssetBundleLoaderMgr>();
	}

    void Start()
    {
        //要加载的资源的队列
        Queue<string> needLoadQueue = new Queue<string>();
        needLoadQueue.Enqueue("prefabs/1");
        needLoadQueue.Enqueue("prefabs/2");
 
        Load(needLoadQueue);
    }
 
    void Load(Queue<string> needLoadQueue)
    {
        if (needLoadQueue.Count > 0)
        {
            string needLoadAssetName = needLoadQueue.Dequeue();
			assetBundleLoaderMgr.LoadAssetBundle(needLoadAssetName, (obj) =>
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                int index = needLoadAssetName.LastIndexOf("/");
                string assetName = needLoadAssetName.Substring(index + 1);
 
                //加载出来的GameObject放到GameObjectPool存储
                GameObjectPool.Add(assetName, go);
 
                Load(needLoadQueue);
            });
        }
        else
        {
            Debug.Log("all finished");
        }
    }
}
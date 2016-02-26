using UnityEngine;
using System.Collections;

public class LoadAssetBundle : MonoBehaviour {

    private string bundlePath;


    void Awake()
    {
        bundlePath = string.Format("file://{0}/", Application.streamingAssetsPath);
    }

	// Use this for initialization
	void Start ()
    {
        //Debug.Log(Application.persistentDataPath);
        unpackageTest();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    public void unpackageTest()
    {
       
       StartCoroutine(LoadBundle("StreamingAssets"));
    }

    IEnumerator LoadBundle(string bundleName)
    {

        //AssetBundle.CreateFromFile()

        WWW mwww = WWW.LoadFromCacheOrDownload(bundlePath + bundleName, 0);
        yield return mwww;

        AssetBundle bundle = mwww.assetBundle;

        AssetBundleManifest mainfest = (AssetBundleManifest)bundle.LoadAsset("AssetBundleManifest");
        bundle.Unload(false);

        string[] dependencyNames = mainfest.GetAllDependencies("prefabs");
		foreach (string n in dependencyNames) {
			Debug.Log ("dependencyNames: "+n);
		}

        AssetBundle[] dependencyBundles = new AssetBundle[dependencyNames.Length];
        for (int i = 0; i < dependencyNames.Length; i++)
        {
            WWW dwww = WWW.LoadFromCacheOrDownload(bundlePath + dependencyNames[i], mainfest.GetAssetBundleHash(dependencyNames[i]));
            yield return dwww;
            dependencyBundles[i] = dwww.assetBundle;
        }

        WWW www = WWW.LoadFromCacheOrDownload(bundlePath + "prefabs", 0);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle ab = www.assetBundle;

			GameObject[] gos = ab.LoadAllAssets<GameObject> ();
			foreach (GameObject go in gos) {
				Debug.Log("Object: " + go.name + "  position: " + go.transform.position);
				Instantiate (go, go.transform.position, go.transform.rotation);
			}
            ab.Unload(false);
        }
		foreach (AssetBundle dependencyBundle in dependencyBundles)
        {
            dependencyBundle.Unload(false);
        }
    }
}

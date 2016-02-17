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
				Instantiate (go);
			}
            ab.Unload(false);
        }
		foreach (AssetBundle dependencyBundle in dependencyBundles)
        {
            dependencyBundle.Unload(false);
        }


        // Load the TextAsset object
        //TextAsset txt = bundle.LoadAsset("TestScript2", typeof(TextAsset)) as TextAsset;

        // Load the assembly and get a type (class) from it
        //var assembly = System.Reflection.Assembly.Load(txt.bytes);
        //var type = assembly.GetType("TestScript2");

        //// Instantiate a GameObject and add a component with the loaded class
        //GameObject go = new GameObject();
        //go.AddComponent(type);

    }
}

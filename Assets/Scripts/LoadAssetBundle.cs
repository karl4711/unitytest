using UnityEngine;
using System.Collections;

public class LoadAssetBundle : MonoBehaviour {

	// Use this for initialization
	void Start () {

        unpackageTest();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void unpackageTest()
    {
        StartCoroutine(LoadBundle("package3.assetbundle"));
    }

    IEnumerator LoadBundle(string bundlePath)
    {

        //AssetBundle.CreateFromFile()

        WWW www = new WWW(string.Format("file://{0}/{1}", Application.streamingAssetsPath, bundlePath));
        yield return www;

        AssetBundle bundle = www.assetBundle;
        Object[] objs = bundle.LoadAll();
        foreach (Object obj in objs)
        {
            Debug.Log(obj.name + ": " + obj.GetType());
        }


        if (www.error != null)
        {
            Debug.LogError("Load Bundle Failed " + bundlePath + " Error Is " + www.error);
            yield break;
        }
        //Do something ...
    }
}

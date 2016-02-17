using UnityEngine;
using UnityEditor;

public class AssetBundleTest : MonoBehaviour
{


    [MenuItem("AssetBundleTest/Package Bundle")]
    public static void package()
    {

        string path = Application.streamingAssetsPath;
        Debug.Log("path: " + path);
        BuildPipeline.BuildAssetBundles(path);
    }

}

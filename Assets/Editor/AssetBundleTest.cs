using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class AssetBundleTest : MonoBehaviour
{


    [MenuItem("AssetBundleTest/Package Bundle")]
    public static void package()
    {

        string path = Application.streamingAssetsPath;
        BuildPipeline.PushAssetDependencies();

        BuildTarget target = BuildTarget.StandaloneWindows;

        BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath("Assets/Textures/0670.png"), null,
                                       path + "/package1.assetbundle",
                                       BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
                                       | BuildAssetBundleOptions.DeterministicAssetBundle, target);


        BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath("Assets/Materials/LogoFlash.mat"), null,
                                       path + "/package2.assetbundle",
                                       BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
                                       | BuildAssetBundleOptions.DeterministicAssetBundle, target);


        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/1.prefab"), null,
                                       path + "/package3.assetbundle",
                                       BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
                                       | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);
        BuildPipeline.PopAssetDependencies();



        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/2.prefab"), null,
                                       path + "/package4.assetbundle",
                                       BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
                                       | BuildAssetBundleOptions.DeterministicAssetBundle, target);
        BuildPipeline.PopAssetDependencies();

        BuildPipeline.PopAssetDependencies();
    }


    

}

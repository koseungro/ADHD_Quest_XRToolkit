using UnityEngine;
using UnityEditor;

public class AssetBundle : MonoBehaviour {

    [MenuItem("Bundles/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {

        string path = Application.dataPath + "/AssetBundles/";
        //Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //for(int i = 0; i < selection.Length; i++)
        //{
        //    Debug.Log("Path : " + selection[i].ToString() + ", " + AssetDatabase.GetAssetPath(selection[i]));
        //}
        /*********************************************************************** \
         * 이름 : BuildPipeLine.BuildAssetBundles() 
         * 용도 : BuildPipeLine 클래스의 함수 BuildAssetBundles()는 에셋번들을 만들어줍니다.
         * 매개변수에는 String 값을 넘기게 되며, 빌드된 에셋 번들을 저장할 경로입니다.
         * 예를 들어 Assets 하위 폴더에 저장하려면 "Assets/AssetBundles"로 입력해야합니다. 
         * ***********************************************************************/
        //GameObject[] selected = Selection.gameObjects;
        //AssetBundleBuild[] bundles = new AssetBundleBuild[selected.Length];
        //for(int i = 0; i < bundles.Length; i++)
        //{
        //    bundles[i].assetBundleName = selected[i].name;
        //    bundles[i].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(selected[i].name);
        //}
        BuildTarget target = BuildTarget.Android;
        BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;
        


        BuildPipeline.BuildAssetBundles(
            path,             
            options,
            target);
        
    }
    
    public void GetBundlesName(string bundleName)
    {
        AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
    }
}


using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAllAtOnce : MonoBehaviour
{
    public AssetReference AssetReference;
    public AssetLabelReference assetLabelReference;
    private GameObject test;
    private GameObject test2;

    // Start is called before the first frame update



    //// Old method used working but all are download
    //public void StartDownload()
    //{
    //    Stopwatch _Time1 = new Stopwatch();
    //    _Time1.Start();
    //    //Whatever needs timing here
    //   AssetReference.InstantiateAsync();
    //    _Time1.Stop();
    //    UnityEngine.Debug.Log(string.Format("Loading Asset: " + AssetReference + "took {0} ms to complete", _Time1.ElapsedMilliseconds));
    //}
    private void Start()
    {
        UnityEngine.Caching.ClearCache();
    }
    // Working using Label - Does not require redownload --- 
    //public void StartDownload()
    //{
    //    Addressables.LoadAssetAsync<GameObject>(assetLabelReference).Completed +=
    //        (AsyncOperationHandle<GameObject> goHandle) =>
    //        {
    //            if (goHandle.Status == AsyncOperationStatus.Succeeded)
    //            {
    //                _AssetDownloaded = Instantiate(goHandle.Result);
    //               // _AssetDownloaded = goHandle.Result;
    //            }
    //            else
    //            {
    //                UnityEngine.Debug.Log("Failed to Load");
    //            }
    //        };
    //}


    //***** Download Individual after recongnizing then delete after destroying-- requires web connection to redownload  //*****

    AsyncOperationHandle<GameObject> opHandle;

    public IEnumerator StartDownload()
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        //Whatever needs timing here
        opHandle = Addressables.LoadAssetAsync<GameObject>(assetLabelReference);
        yield return opHandle;

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            test = opHandle.Result;
            test2 = Instantiate(test, transform);
            UnityEngine.Debug.Log(string.Format("Loading Asset: " + test2.name + "took {0} ms to complete", st.ElapsedMilliseconds));


        }
    }

    public void OnDestroy2()
    {
        Addressables.Release(opHandle);
        Addressables.ReleaseInstance(test);
        //   UnityEngine.Caching.ClearCache();

        Resources.UnloadUnusedAssets();
        DestroyImmediate(test, true);
        Destroy(test2);
    }

    public void StartDownloading()
    {
        StartCoroutine(StartDownload());
    }
    //*****  //*****  //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //***** //*****

    public void Release()
    {
        Destroy(test);

        UnityEngine.Debug.Log("Object Realesed");
        //AssetReference.ReleaseAsset();
        Addressables.ReleaseInstance(test);
        //    Addressables.Release(_AssetDownloaded);
        UnityEngine.Caching.ClearCache();

        Resources.UnloadUnusedAssets();

        //  Addressables.ClearDependencyCacheAsync();
    }

    //[MenuItem("Tools/clear addressables cache", false, 50)]
    public static void ClearAddressablesCache()
    {
        UnityEngine.Caching.ClearCache();
    }
}

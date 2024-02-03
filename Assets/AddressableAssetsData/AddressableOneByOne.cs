using System.Collections;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableOneByOne : MonoBehaviour
{
    public TMPro.TMP_Dropdown _DropDownAssetSelection;
    public ToggleGroup toggleGroupInstance_Comp;
    public TMPro.TMP_Text _InfoSelection;
    [SerializeField] string assetLabelReference;
    AsyncOperationHandle<GameObject> handle;
    private GameObject _AssetDownloaded;
    private GameObject _AssetInstantce;
    string statsText;
    string statsText2;
    Stopwatch _Time1 = new Stopwatch();
    Stopwatch _Time2 = new Stopwatch();
    Stopwatch _Timer3 = new Stopwatch();
    Stopwatch _Timer5 = new Stopwatch();

    GUIStyle style;
    private string log = "";
    private bool _LZMA, _LZ4;
    private string extention;
    private bool _downloadDone = false;
    private GameObject _AssetInstantceClone;
    private bool _FirstDispayed;

    public Toggle currentSelection_Asset
    {
        get { return toggleGroupInstance_Comp.ActiveToggles().FirstOrDefault(); }
    }

    // Display value of the dropdown on change
    private void DropdownValueChanged(TMP_Dropdown change)
    {
        _ChangeStart();
    }

    private void Update()
    {
        _ChangeStart();
    }

    private void _ChangeStart()
    {
        if (currentSelection_Asset.name == "U.Lzma") _LZMA = true; else _LZMA = false;
        if (currentSelection_Asset.name == "U.Lz4") _LZ4 = true; else _LZ4 = false;


        if (_LZMA) extention = ".lzma";
        if (_LZ4) extention = ".lz4";
        if (!_LZMA && !_LZ4) extention = "";

        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "rotary-machine_final_1") assetLabelReference = "Lab1";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_1") assetLabelReference = "Lab4";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_larger1") assetLabelReference = "Lab5";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_larger2") assetLabelReference = "Lab6";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_larger3") assetLabelReference = "Lab7";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_medium") assetLabelReference = "Lab2";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "packing_sim_final_larger0") assetLabelReference = "Lab3";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "robot_factory_final_0") assetLabelReference = "Lab8";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "robot_factory_final_1") assetLabelReference = "Lab9";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "robot_factory_final_2") assetLabelReference = "Lab10";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "robot_factory_final_3") assetLabelReference = "Lab11";
        if (_DropDownAssetSelection.options[_DropDownAssetSelection.value].text == "robot_factory_final_4") assetLabelReference = "Lab12";

        _InfoSelection.text = "Seletion:  " + _DropDownAssetSelection.options[_DropDownAssetSelection.value].text + "  [" + assetLabelReference + "]  " + extention;


    }
    void Start()
    {
        DropdownValueChanged(_DropDownAssetSelection);
        _DropDownAssetSelection.onValueChanged.AddListener(delegate { DropdownValueChanged(_DropDownAssetSelection); });

        Caching.ClearCache();
        UnityEngine.Caching.ClearCache();
        statsText = string.Format("Placing Asset Time (ms): ");
        statsText2 = string.Format("Complete Loading Asset Time (ms): ");
        Addressables.ClearDependencyCacheAsync("default");
        style = new GUIStyle();
        style.richText = true;
        GUI.backgroundColor = Color.black;
        style.normal.background = Texture2D.grayTexture;
    }


    public void StartDownloading2()
    {

        StartCoroutine(StartNEW());
    }

    public IEnumerator StartNEW()
    {
        if (!_downloadDone)
        {
            // Download the file from the URL. It will not be saved in the Cache
            // UnityEngine.Debug.Log("Downloading " + extention + " file...");
            log += "  " + "<color=#FF7F33>Downloading " + "ADDR" + extention + " file ...\n</color>";
            Profiler.BeginSample("Download");
            _Time2.Start();

            // Clear all cached AssetBundles
            // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
            // Addressables.ClearDependencyCacheAsync(key);
            _Timer3.Start();
            //Check the download size
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(assetLabelReference);
            yield return getDownloadSize;


            //If the download size is greater than 0, download all the dependencies.
            if (getDownloadSize.Result > 0)
            {

                AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(assetLabelReference);
                yield return downloadDependencies;



                if (downloadDependencies.Status == AsyncOperationStatus.Succeeded)
                {

                    _Timer3.Stop();
                    log += "  " + "<color=#00CCCC>Downloading Time: </color>" + "<color=white>" + _Timer3.ElapsedMilliseconds + " msec \n</color>";
                    Profiler.EndSample();
                    _downloadDone = true;
                }

                Addressables.Release(downloadDependencies); //Release the operation handle


            }


        }

        if (_FirstDispayed)
        {
            _Time2.Start();
            _Timer5.Start();
            handle = Addressables.LoadAssetAsync<GameObject>(assetLabelReference);
            yield return handle;
            _Timer5.Stop();
            log += "  " + "<color=yellow>Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

            if (handle.Result != null)
                Profiler.BeginSample("AssetbundleLoadasset_First");
            _Time1.Start();
            _AssetInstantceClone = Instantiate(handle.Result, transform) as GameObject;
            //  Addressables.Release(handle);
            Profiler.EndSample();
            _Time1.Stop();
            _Time2.Stop();
            statsText += string.Format("\n" + _DropDownAssetSelection.options[_DropDownAssetSelection.value].text + ": {0} ", _Time1.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + _DropDownAssetSelection.options[_DropDownAssetSelection.value].text + ": {0} ", _Time2.ElapsedMilliseconds);

        }

        if (!_FirstDispayed)
        {

            _Timer5.Start();
            handle = Addressables.LoadAssetAsync<GameObject>(assetLabelReference);
            yield return handle;
            _Timer5.Stop();
            log += "  " + "<color=yellow>Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

            if (handle.Result != null)
                Profiler.BeginSample("AssetbundleLoadasset_First");
            _Time1.Start();
            _AssetInstantceClone = Instantiate(handle.Result, transform) as GameObject;
            Profiler.EndSample();
            _Time1.Stop();
            _Time2.Stop();
            statsText += string.Format("\n" + _DropDownAssetSelection.options[_DropDownAssetSelection.value].text + ": {0} ", _Time1.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + _DropDownAssetSelection.options[_DropDownAssetSelection.value].text + ": {0} ", _Time2.ElapsedMilliseconds);
            _FirstDispayed = true;
        }

    }


    //...


    public void Release()
    {
        Profiler.BeginSample("Release");
        _Time1.Reset();
        _Time2.Reset();
        _Timer5.Reset();
        //    statsText = string.Format("Placing Asset Time (ms): ");
        //    statsText2 = string.Format("Complete Loading Asset Time (ms): ");
        Resources.UnloadUnusedAssets();
        DestroyImmediate(_AssetInstantceClone, true);
        Profiler.EndSample();
        // Used to free memory which require reloading the bunlde and decompressing it
        Addressables.Release(handle);

        //  Destroy(_AssetInstantce);
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 100, 330, 65), statsText);
        GUI.TextArea(new Rect(10, 165, 330, 65), statsText2);
        GUI.TextArea(new Rect(10, 250, 330, 70), log, style);
    }

}



//public void StartDownloading()
//{

//    StartCoroutine(StartDownload());
//}

//public IEnumerator StartDownload()
//{
//    Profiler.BeginSample("Download");
//    _Timer3.Start();
//    _Time2.Start();
//    opHandle = Addressables.LoadAssetAsync<GameObject>(assetLabelReference);
//    yield return opHandle;

//    if (opHandle.Status == AsyncOperationStatus.Succeeded)
//    {
//        _AssetDownloaded = opHandle.Result;

//        _Timer3.Stop();
//        log += "  " + "<color=#00CCCC>Downloading Time: </color>" + "<color=white>" + _Timer3.ElapsedMilliseconds + " msec \n</color>";
//        _Time1.Start();
//        Profiler.EndSample();
//        Profiler.BeginSample("AssetbundleLoadasset_First");
//       // _AssetInstantce = Instantiate(_AssetDownloaded, transform);
//        _Time1.Stop();
//        _Time2.Stop();
//        UnityEngine.Debug.Log(string.Format("Loading Asset: " + _AssetInstantce.name + "took {0} ms to complete", _Time1.ElapsedMilliseconds));
//        statsText = string.Format("\n" + _AssetInstantce.name + ": {0} ", _Time1.ElapsedMilliseconds);
//        statsText2 = string.Format("\n" + _AssetInstantce.name + ": {0} ", _Time2.ElapsedMilliseconds);
//        Profiler.EndSample();
//    }
//}
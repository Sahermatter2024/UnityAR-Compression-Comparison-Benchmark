using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Vuforia;

class NonCachingLoad1 : MonoBehaviour
{
    public TMPro.TMP_Dropdown _DropDownAssetSelection;
    public ToggleGroup toggleGroupInstance_Comp;
    public TMPro.TMP_Text _InfoSelection;
    public string BundleURL;
    public string myFile;
    private GameObject mBundleInstance = null;
    protected ObserverBehaviour mTrackableBehaviour;
    public int version = 0;
    private AssetBundle bundle;
    private string ppath;
    string statsText;
    Stopwatch _Timer1 = new Stopwatch();
    Stopwatch _Timer2 = new Stopwatch();
    Stopwatch _Timer3 = new Stopwatch();
    Stopwatch _Timer5 = new Stopwatch();
    GUIStyle style;
    private string log = "";
    private string statsText2;
    private bool _LZMA, _LZ4;
    private string extention;
    AssetBundle _AssetDownloaded;
    private GameObject _AssetInstantce;
    private GameObject _AssetInstantceClone;
    private bool _donwnloadDone = false;
    private bool _FirstDispayed = false;
    private bool downloadDone;
    private bool FirstDone = false;
    private bool _SeconedDisplayed;
    private AssetBundle myLoadedAssetBundle;
    private AssetBundleCreateRequest bundleLoadRequest;
    private AssetBundleRequest assetLoadRequest;

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
        _InfoSelection.text = "Seletion:  " + myFile + extention;
    }

    private void _ChangeStart()
    {
        if (currentSelection_Asset.name == "U.Lzma") _LZMA = true; else _LZMA = false;
        if (currentSelection_Asset.name == "U.Lz4") _LZ4 = true; else _LZ4 = false;
        if (_LZMA) extention = ".lzma";
        if (_LZ4) extention = ".lz4";
        if (!_LZMA && !_LZ4) extention = "";

        if (_LZ4) BundleURL = "http://192.168.70.142:8080/StreamingAssets/final/Lz4/" + myFile;
        if (_LZMA) BundleURL = "http://192.168.70.142:8080/StreamingAssets/final/Lzma/" + myFile;
        if (!_LZMA && !_LZ4) BundleURL = "";
        _InfoSelection.text = "Seletion:  " + myFile + extention;
        myFile = _DropDownAssetSelection.options[_DropDownAssetSelection.value].text;
    }


    void Start()
    {
        ppath = Application.persistentDataPath;
        statsText = string.Format("Placing Asset Time (ms): ");
        statsText2 = string.Format("Complete Loading Asset Time (ms): ");
        DropdownValueChanged(_DropDownAssetSelection);
        _DropDownAssetSelection.onValueChanged.AddListener(delegate { DropdownValueChanged(_DropDownAssetSelection); });

        UnityEngine.Caching.ClearCache();
        if (Directory.Exists(ppath)) { Directory.Delete(ppath, true); }
        Directory.CreateDirectory(ppath);
        Caching.ClearAllCachedVersions(myFile);
        Caching.ClearCache();
        style = new GUIStyle();
        style.richText = true;
        GUI.backgroundColor = Color.black;
        style.normal.background = Texture2D.grayTexture;
    }

    public AsyncCompletedEventHandler DownloadFileCompleted(string filename)
    {
        Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
        {
            var _filename = filename;
            if (e.Error != null)
            {
                throw e.Error;
            }
            //   log += "  " + "<color=#00CCCC>Memory used before full collection: </color>" + "<color=white>" + GC.GetTotalMemory(false) + " msec \n</color>";
            Profiler.EndSample();
            _donwnloadDone = true;
            log += "  " + "<color=green>Completed!</color>\n";
            _Timer3.Stop();
            log += "  " + "<color=#00CCCC>Downloading Time: </color>" + "<color=white>" + _Timer3.ElapsedMilliseconds + " msec \n</color>";

            GC.Collect();
            //   GC.WaitForPendingFinalizers();

            StartCoroutine(Download7ZFile3());

        };

        //   log += "  " + "<color=#00CCCC>Memory used before dOWNLOADING: </color>" + "<color=white>" + GC.GetTotalMemory(true) + " msec \n</color>";

        return new AsyncCompletedEventHandler(action);
    }

    IEnumerator Download()
    {
        if (!_donwnloadDone)
        {
            _Timer3.Start();
            _Timer2.Start();
            while (!Caching.ready)
                yield return null;
            log += "  " + "<color=#FF7F33>Downloading " + "ASSBUN_D" + extention + " file ...\n</color>";
            Profiler.BeginSample("Download");
            using (var client = new WebClient())
            {
                Profiler.BeginSample("Download File");
                client.DownloadFileCompleted += DownloadFileCompleted(myFile);
                client.DownloadFileAsync(new System.Uri(BundleURL), ppath + "/" + myFile);
                client.Dispose();
                yield return true;

            }

        }

    }

    IEnumerator Download7ZFile3()
    {

        {
            StartCoroutine(AssetbundleLoadasset_First());
        }
        yield return null;
    }

    IEnumerator AssetbundleLoadasset_First()
    {

        if (_FirstDispayed)
        {
            Profiler.BeginSample("AssetbundleLoadasset_Second");
            _Timer2.Start();
            _Timer5.Start();
            bundleLoadRequest = AssetBundle.LoadFromFileAsync(ppath + "/" + myFile);
            yield return bundleLoadRequest;

            myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            bundleLoadRequest = null;

            if (myLoadedAssetBundle == null)
            {
                UnityEngine.Debug.Log("Failed to load AssetBundle!");
                FirstDone = false;
                yield break;
            }


            assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(myFile);
            if (null == assetLoadRequest)
            {
                UnityEngine.Debug.Log("Failed to load AssetBundle: " + ppath + "/" + myFile);
                FirstDone = false;
                yield break;
            }
            yield return assetLoadRequest;

            _Timer5.Stop();
            log += "  " + "<color=yellow>Bundle Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

            _Timer1.Stop();
            _AssetInstantceClone = Instantiate(assetLoadRequest.asset, transform) as GameObject;
            //  Instantiate(prefab);
            assetLoadRequest = null; ;

            _Timer1.Stop();
            _Timer2.Stop();
            GC.Collect();
            Resources.UnloadUnusedAssets();
            FirstDone = true;

            statsText2 += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Timer2.ElapsedMilliseconds);
            statsText += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Timer1.ElapsedMilliseconds);
            Profiler.EndSample();
        }

        if (!_FirstDispayed)
        {
            Profiler.BeginSample("AssetbundleLoadasset_First");
            _Timer5.Start();
            bundleLoadRequest = AssetBundle.LoadFromFileAsync(ppath + "/" + myFile);
            yield return bundleLoadRequest;

            myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            bundleLoadRequest = null;
            if (myLoadedAssetBundle == null)
            {
                UnityEngine.Debug.Log("Failed to load AssetBundle!");
                FirstDone = false;
                yield break;
            }

            assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(myFile);
            if (null == assetLoadRequest)
            {
                UnityEngine.Debug.Log("Failed to load AssetBundle: " + ppath + "/" + myFile);
                FirstDone = false;
                yield break;
            }
            yield return assetLoadRequest;
            _Timer5.Stop();
            log += "  " + "<color=yellow>Bundle Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

            _Timer1.Start();
            _AssetInstantceClone = Instantiate(assetLoadRequest.asset, transform) as GameObject;
            _Timer1.Stop();
            _Timer2.Stop();
            assetLoadRequest = null; ;
            AssetBundle.UnloadAllAssetBundles(false);
            GC.Collect();
            _FirstDispayed = true;


            statsText2 += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Timer2.ElapsedMilliseconds);
            statsText += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Timer1.ElapsedMilliseconds);
            Profiler.EndSample();
        }
    }


    public void StartDownload()
    {
        ;
        StartCoroutine(Download());
        if (_FirstDispayed) StartCoroutine(Download7ZFile3());


    }

    //  Unload an AssetBundle
    public void Unload()
    {
        Profiler.BeginSample("Release");

        _Timer1.Reset();
        _Timer2.Reset();
        _Timer5.Reset();
        Resources.UnloadUnusedAssets();
        DestroyImmediate(_AssetInstantceClone);
        AssetBundle.UnloadAllAssetBundles(true);
        GC.Collect();
        Profiler.EndSample();

    }
    void OnGUI()
    {
        //  GUI.contentColor = Color.red;
        GUI.TextArea(new Rect(10, 100, 330, 65), statsText);
        GUI.TextArea(new Rect(10, 165, 330, 65), statsText2);
        GUI.TextArea(new Rect(10, 250, 330, 70), log, style);
    }
}


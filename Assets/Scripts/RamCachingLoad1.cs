using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class RamCachingLoad1 : MonoBehaviour
{
    public TMPro.TMP_Dropdown _DropDownAssetSelection;
    public ToggleGroup toggleGroupInstance_Comp;
    public TMPro.TMP_Text _InfoSelection;
    public string bundleURL;
    public string myFile;
    private GameObject mBundleInstance = null;
    private GameObject _AssetInstantceClone;

    private AssetBundle bundle;
    private bool mAttached = false;
    string statsText;
    string statsText2;
    private string extention;
    GUIStyle style;
    private string log = "";
    Stopwatch st = new Stopwatch();
    Stopwatch st2 = new Stopwatch();
    Stopwatch _Timer3 = new Stopwatch();
    Stopwatch _Timer5 = new Stopwatch();

    private bool _LZMA, _LZ4;
    private bool _downloadDone = false;
    private bool _FirstDispayed = false;

    public Toggle currentSelection_Asset
    {
        get { return toggleGroupInstance_Comp.ActiveToggles().FirstOrDefault(); }
    }

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

        if (_LZ4) bundleURL = "http://192.168.70.142:8080/StreamingAssets/final/Lz4/" + myFile;
        if (_LZMA) bundleURL = "http://192.168.70.142:8080/StreamingAssets/final/Lzma/" + myFile;
        if (!_LZMA && !_LZ4) bundleURL = "";
        _InfoSelection.text = "Seletion:  " + myFile + extention;
        myFile = _DropDownAssetSelection.options[_DropDownAssetSelection.value].text;


    }
    void Start()
    {
        statsText = string.Format("Placing Asset Time (ms): ");
        statsText2 = string.Format("Complete Loading Asset Time (ms): ");
        DropdownValueChanged(_DropDownAssetSelection);
        _DropDownAssetSelection.onValueChanged.AddListener(delegate { DropdownValueChanged(_DropDownAssetSelection); });
        style = new GUIStyle();
        style.richText = true;
        GUI.backgroundColor = Color.black;
        style.normal.background = Texture2D.grayTexture;
        UnityEngine.Caching.ClearCache();
        Caching.ClearCache();

    }



    // Update is called once per frame

    IEnumerator DownloadAndCache()
    {
        if (!_downloadDone)
        {
            log += "  " + "<color=#FF7F33>Downloading " + "ASSBUN_R" + extention + " file ...\n</color>";
            _Timer3.Start();
            st2.Start();
            if (bundle != null)
            {
                bundle.Unload(false); //scene is unload from here
            }

            while (!Caching.ready)
                yield return null;

            using (UnityWebRequest mAssetBundle = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
            {
                Profiler.BeginSample("Download");
                if (bundle != null)
                {
                    bundle.Unload(false); //scene is unload from here
                }
                if (!mBundleInstance)
                {
                    yield return mAssetBundle.SendWebRequest();

                    if (mAssetBundle.error != null)
                        throw new UnityException("mAssetBundle download had an error: " + mAssetBundle.error);


                    bundle = DownloadHandlerAssetBundle.GetContent(mAssetBundle);
                    mAssetBundle.Dispose();
                    _Timer3.Stop();
                    log += "  " + "<color=#00CCCC>Downloading Time: </color>" + "<color=white>" + _Timer3.ElapsedMilliseconds + " msec \n</color>";
                    _downloadDone = true;
                    Profiler.EndSample();
                }
            }


        }

        yield return null;
        //   bundle.Unload(false);
    }

    bool Downloaded()
    {
        return _downloadDone;
    }

    IEnumerator Download7ZFile3()
    {
        yield return new WaitUntil(Downloaded);

        StartCoroutine(AssetbundleLoadasset_First());

        yield return true;
    }


    IEnumerator AssetbundleLoadasset_First()
    {

        if (_FirstDispayed)
        {
            Profiler.BeginSample("AssetbundleLoadasset_Second");

            _AssetInstantceClone = Instantiate(mBundleInstance, transform);

            st.Stop();
            st2.Stop();
            statsText += string.Format("\n" + myFile + ": {0} ", st.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + myFile + ": {0} ", st2.ElapsedMilliseconds);
            Profiler.EndSample();
            yield return null;
        }

        if (!_FirstDispayed)
        {
            Profiler.BeginSample("AssetbundleLoadasset_First");

            _Timer5.Start();
            st.Start();
            mBundleInstance = bundle.LoadAsset(myFile) as GameObject;
            _Timer5.Stop();
            log += "  " + "<color=yellow>Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";


            _AssetInstantceClone = Instantiate(mBundleInstance, transform);
            GC.Collect();
            st.Stop();
            st2.Stop();
            statsText += string.Format("\n" + myFile + ": {0} ", st.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + myFile + ": {0} ", st2.ElapsedMilliseconds);
            Profiler.EndSample();
            _FirstDispayed = true;
            yield return null;
        }
    }

    void OnGUI()
    {
        //  GUI.contentColor = Color.red;
        GUI.TextArea(new Rect(10, 100, 330, 65), statsText);
        GUI.TextArea(new Rect(10, 165, 330, 65), statsText2);
        GUI.TextArea(new Rect(10, 250, 330, 70), log, style);
    }

    //   private Rigidbody rb;
    public void LoadAssetToTarget()
    {

        if (!mBundleInstance)
        {
            Profiler.BeginSample("AssetbundleLoadasset_First");

            _Timer5.Start();
            st.Start();
            mBundleInstance = bundle.LoadAsset(myFile) as GameObject;
            _Timer5.Stop();
            log += "  " + "<color=yellow>Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

            _AssetInstantceClone = Instantiate(mBundleInstance, transform);
            mBundleInstance = null;
            //  AssetBundle.UnloadAllAssetBundles(false);

            GC.Collect();
            st.Stop();
            st2.Stop();
            statsText += string.Format("\n" + myFile + ": {0} ", st.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + myFile + ": {0} ", st2.ElapsedMilliseconds);
            Profiler.EndSample();

        }
        else
        {
            Profiler.BeginSample("AssetbundleLoadasset_Second");


            mBundleInstance.transform.gameObject.SetActive(true);

            st.Stop();
            st2.Stop();
            statsText += string.Format("\n" + myFile + ": {0} ", st.ElapsedMilliseconds);
            statsText2 += string.Format("\n" + myFile + ": {0} ", st2.ElapsedMilliseconds);
            Profiler.EndSample();

        }
    }

    public void StartDownload()
    {
        if (!_downloadDone) StartCoroutine(DownloadAndCache());
        StartCoroutine(Download7ZFile3());
    }

    //  Unload an AssetBundle
    public void Unload()
    {
        Profiler.BeginSample("Release");
        st.Reset();
        _Timer5.Reset();
        GC.Collect();
        Profiler.EndSample();
        DestroyImmediate(_AssetInstantceClone, true);
    }
}
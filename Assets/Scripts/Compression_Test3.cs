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

public class Compression_Test3 : MonoBehaviour
{
    public TMPro.TMP_Dropdown _DropDownAssetSelection;
    public ToggleGroup toggleGroupInstance_Comp;
    public TMPro.TMP_Text _InfoSelection;
    string statsText;
    string statsText2;
    Stopwatch _Time1 = new Stopwatch();
    Stopwatch _Time2 = new Stopwatch();
    Stopwatch _Time3 = new Stopwatch();
    Stopwatch _Timer5 = new Stopwatch();

    private int lzres = 0, zipres = 0, flzres = 0;
    private int brres = 0, lz4res = 0, gzres = 0;

    //for counting the time taken to decompress the 7z file.
    private float t1, tim;

    //the _AssetDownloaded file to download.
    public string myFile = "rotary-machine_final_1.7z";

    //the adress from where we download our _AssetDownloaded file
    private string uri = "http://192.168.70.142:8080/StreamingAssets/Prefabs/Complete%20Data/";

    private string ppath;

    private string log = "";

    private bool downloadDone, decompressedDone;

    GUIStyle style;

    //A 1 item integer array to get the current extracted file of the 7z archive. Compare this to the total number of the files to get the progress %.
    private int[] progress = new int[1];
    private ulong[] progress1 = new ulong[1];
    private ulong[] progress2 = new ulong[1];
    private float[] progress3 = new float[1];
    private ulong[] progress4 = new ulong[1];
    private ulong[] bytes = new ulong[1];
    private ulong[] gzProgress = new ulong[1];

    private AssetBundle myLoadedAssetBundle;
    private GameObject _AssetDownloaded;
    private GameObject _AssetInstantce;
    private GameObject _AssetInstantceClone;

    private bool _Uncompressed = false;

    private bool _7Zip = false;
    private bool _LZMA = false;
    private bool _LZ4 = false;
    private bool _FastLZ = false;
    private bool _Gzip = false;
    private bool _Zip = false;
    private bool _Brotli = false;
    private bool _ResourceFolder = false;
    private bool _Assetbundle = true;

    private string extention;
    //  Thread th1, th2;
    private bool FirstDone = false;

    public Toggle currentSelection_Comp
    {
        get { return toggleGroupInstance_Comp.ActiveToggles().FirstOrDefault(); }
    }

    // Display value of the dropdown on change
    private void DropdownValueChanged(TMP_Dropdown change)
    {
        myFile = _DropDownAssetSelection.options[_DropDownAssetSelection.value].text;
        _ChangeStart();
    }

    private void Update()
    {
        _ChangeStart();
    }

    private void _ChangeStart()
    {
        if ((_ResourceFolder) && (_Brotli || _Gzip || _LZMA || _LZ4 || _FastLZ))
        {
            myFile = myFile + ".prefab";
        }
        if (_Brotli) extention = ".br";
        if (_Zip) extention = ".zip";
        if (_Gzip) extention = ".gz";
        if (_7Zip) extention = ".7z";
        if (_LZMA) extention = ".lzma";
        if (_LZ4) extention = ".lz4";
        if (_FastLZ) extention = ".flz";
        if (_Uncompressed) extention = "";
        if (currentSelection_Comp.name == "Uncompressed") _Uncompressed = true; else _Uncompressed = false;
        if (currentSelection_Comp.name == "7z") _7Zip = true; else _7Zip = false;
        if (currentSelection_Comp.name == "Lzma") _LZMA = true; else _LZMA = false;
        if (currentSelection_Comp.name == "Lz4") _LZ4 = true; else _LZ4 = false;
        if (currentSelection_Comp.name == "FastLz") _FastLZ = true; else _FastLZ = false;
        if (currentSelection_Comp.name == "Gzip") _Gzip = true; else _Gzip = false;
        if (currentSelection_Comp.name == "Zip") _Zip = true; else _Zip = false;
        if (currentSelection_Comp.name == "Brotli") _Brotli = true; else _Brotli = false;

        _InfoSelection.text = "Seletion:  " + myFile + extention;

    }

    private void Start()
    {

        UnityEngine.Caching.ClearCache();
        statsText = string.Format("Placing Asset Time (ms): ");
        statsText2 = string.Format("Complete Loading Asset Time (ms): ");
        DropdownValueChanged(_DropDownAssetSelection);
        _DropDownAssetSelection.onValueChanged.AddListener(delegate { DropdownValueChanged(_DropDownAssetSelection); });


        ppath = Application.persistentDataPath;

        lzma.persitentDataPath = Application.persistentDataPath;

        if (Directory.Exists(ppath)) { Directory.Delete(ppath, true); }
        Directory.CreateDirectory(ppath);


        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        decompressedDone = false;
        style = new GUIStyle();
        style.richText = true;
        GUI.backgroundColor = Color.black;
        style.normal.background = Texture2D.grayTexture;

    }
    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 100, 350, 40), statsText);
        GUI.TextArea(new Rect(10, 140, 350, 60), statsText2);
        lzres = 0; zipres = 0; flzres = 0; lz4res = 0;
        GUI.TextArea(new Rect(10, 200, 350, 70), log, style);


    }

    private void _FirstDuration()
    {
        _Time1.Start();
        _AssetDownloaded = Resources.Load<GameObject>(myFile);
        _AssetInstantce = Instantiate(_AssetDownloaded, transform);
        _Time1.Stop();
        _Time2.Stop();
        statsText2 += string.Format("\n" + _AssetDownloaded.name + ": {0} ", _Time2.ElapsedMilliseconds);
        statsText += string.Format("\n" + _AssetDownloaded.name + ": {0} ", _Time1.ElapsedMilliseconds);
    }

    private void _SecondDuration()
    {
        _Time1.Start();
        _AssetDownloaded = Resources.Load<GameObject>(myFile);
        _AssetInstantce = Instantiate(_AssetDownloaded, transform);
        _Time1.Stop();
        statsText += string.Format("\n" + _AssetDownloaded.name + ": {0} ", _Time1.ElapsedMilliseconds);
    }




    IEnumerator decompressFunc()
    {
        Stopwatch _Time5 = new Stopwatch();


        if (_7Zip && !decompressedDone)
        {

            Profiler.BeginSample("Decompression _7Zip");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing 7zip ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            _Time5.Start();
            lzres = lzma.doDecompress7zip(ppath + "/" + myFile + extention, ppath + "/", true, true);
            log += "  <color=black>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
            _Time5.Stop();
            log += "  " + "<color=red>Status: " + lzres + " |  7z Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>\n";
            decompressedDone = true;
            Profiler.EndSample();
            if (File.Exists(ppath + "/" + myFile + extention)) File.Delete(ppath + "/" + myFile + extention);
            yield return true;

        }


        if (_Uncompressed && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _Uncompressed");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Uncompressed Size ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)\n</color></color>";
            log += "  " + "<color=red>Status: 1 " + "| time: </color>" + "<color=yellow>" + "0 sec" + " </color>\n\n";
            Profiler.EndSample();
            decompressedDone = true;
            yield return true;

        }



        if (_Zip && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _Zip");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing Zip ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            _Time5.Start();
            zipres = lzip.decompress_File(ppath + "/" + myFile + extention, ppath + "/", progress, null, progress1);
            _Time5.Stop();
            log += "  <color=black>(" + progress1[0].ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + zipres + " |  zip Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>\n";
            decompressedDone = true;
            Profiler.EndSample();
            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
            yield return true;

        }



        if (_LZMA && !decompressedDone)
        {

            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing Lzma ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            Profiler.BeginSample("Decompression _LZMA");
            _Time5.Start();
            lzres = lzma.LzmaUtilDecode(ppath + "/" + myFile + ".lzma", ppath + "/" + myFile);
            _Time5.Stop();
            Profiler.EndSample();
            log += "  <color=black>(" + lzma.getBytesWritten().ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + lzres + " |  Lzma Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>  \n";
            decompressedDone = true;
            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
            yield return true;

        }

        if (_LZ4 && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _LZ4");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing Lz4 ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            _Time5.Start();
            lz4res = LZ4.decompress(ppath + "/" + myFile + ".lz4", ppath + "/" + myFile, bytes);
            _Time5.Stop();
            log += "<color=black>(" + bytes[0].ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + lz4res + " |  LZ4 Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>  \n";
            decompressedDone = true;
            Profiler.EndSample();
            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
        }

        if (_Gzip && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _Gzip");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing Gzip ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            _Time5.Start();
            gzProgress[0] = 0;
            gzres = lzip.ungzipFile(ppath + "/" + myFile + ".gz", ppath + "/" + myFile, gzProgress);
            _Time5.Stop();
            log += "  <color=black>(" + gzProgress[0].ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + gzres + " |  gzip Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>\n";
            decompressedDone = true;
            Profiler.EndSample();
            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
        }

        if (_Brotli && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _Brotli");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing Brotli ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            progress4[0] = 0;
            _Time5.Start();
            brres = brotli.decompressFile(ppath + "/" + myFile + ".br", ppath + "/" + myFile, progress4);
            _Time5.Stop();
            log += "  <color=black>(" + progress4[0].ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + brres + " |  brotli Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>  \n";
            decompressedDone = true;
            Profiler.EndSample();
            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
            System.GC.Collect();
            yield return true;


        }

        if (_FastLZ && !decompressedDone)
        {
            Profiler.BeginSample("Decompression _FastLZ");
            System.IO.FileInfo fio;
            fio = new FileInfo(ppath + "/" + myFile + extention);
            log += "  " + "<color=green>Decompressing FastLZ ... <color=blue>(" + ((float)fio.Length / 1024).ToString("F") + " kb)</color></color>";
            progress2[0] = 0;
            _Time5.Start();
            flzres = fLZ.decompressFile(ppath + "/" + myFile + ".flz", ppath + "/" + myFile, true, progress2);
            _Time5.Stop();
            log += "  <color=black>(" + progress2[0].ToString() + ")\n</color>";
            log += "  " + "<color=red>Status: " + flzres + " |  flz Decompression time: </color>" + "<color=yellow>" + _Time5.ElapsedMilliseconds + "   msec</color>\n";
            decompressedDone = true;
            Profiler.EndSample();

            if (File.Exists(ppath + "/" + myFile)) File.Delete(ppath + "/" + myFile + extention);
            yield return true;

        }

        StartCoroutine(Download7ZFile3());


    }

    public void Starting()
    {

        if (!File.Exists(ppath + "/" + myFile + extention) && !downloadDone) StartCoroutine(Download7ZFile2());

    }


    private static void DownloadFileCallback2(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Console.WriteLine("File download cancelled.");
        }

        if (e.Error != null)
        {
            Console.WriteLine(e.Error.ToString());
        }
    }

    IEnumerator Download7ZFile2()
    {
        if (File.Exists(ppath + "/" + myFile + extention)) File.Delete(ppath + "/" + myFile + extention);
        _Time3.Start();
        _Time2.Start();
        log += "  " + "<color=#FF7F33>Downloading " + extention + " file ...</color>";

        using (var client = new WebClient())
        {
            Profiler.BeginSample("Download File");
            client.DownloadFileCompleted += DownloadFileCompleted(myFile);
            client.DownloadFileAsync(new System.Uri(uri + myFile + extention), ppath + "/" + myFile + extention);
            yield return client;

        }

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

            Profiler.EndSample();
            downloadDone = true;
            log += "  " + "<color=green>Completed!</color>\n";
            _Time3.Stop();
            log += "  " + "<color=#00CCCC>Downloading Time: </color>" + "<color=white>" + _Time3.ElapsedMilliseconds + " msec \n</color>";
            GC.Collect(0, GCCollectionMode.Forced);
            StartCoroutine(decompressFunc());
        };

        return new AsyncCompletedEventHandler(action);
    }




    IEnumerator Download7ZFile3()
    {

        yield return new WaitUntil(Decompressed);
        if (_Assetbundle && decompressedDone && !FirstDone)
        {
            StartCoroutine(AssetbundleLoadasset_First());
        }
        else if (_Assetbundle && decompressedDone && FirstDone)
        {
            StartCoroutine(AssetbundleLoadasset_Second());
        }
    }
    bool DownloadDone()
    {
        return downloadDone;
    }
    bool Decompressed()
    {
        return decompressedDone;
    }
    private AssetBundleCreateRequest bundleLoadRequest;
    private AssetBundleRequest assetLoadRequest;

    IEnumerator AssetbundleLoadasset_First()
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


        _Time1.Start();
        _AssetInstantceClone = Instantiate(assetLoadRequest.asset, transform) as GameObject;
        _Time1.Stop();
        _Time2.Stop();
        assetLoadRequest = null; ;
        AssetBundle.UnloadAllAssetBundles(false);
        System.GC.Collect();
        FirstDone = true;


        statsText2 += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Time2.ElapsedMilliseconds);
        statsText += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Time1.ElapsedMilliseconds);
        Profiler.EndSample();
    }

    IEnumerator AssetbundleLoadasset_Second()
    {

        Profiler.BeginSample("AssetbundleLoadasset_Second");
        _Time2.Start();
        _Timer5.Start();
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(ppath + "/" + myFile);
        yield return bundleLoadRequest;

        myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            UnityEngine.Debug.Log("Failed to load AssetBundle!");
            FirstDone = false;
            yield break;
        }

        var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(myFile);
        if (null == assetLoadRequest)
        {
            UnityEngine.Debug.Log("Failed to load AssetBundle: " + ppath + "/" + myFile);
            FirstDone = false;
            yield break;
        }
        yield return assetLoadRequest;
        _Timer5.Stop();
        log += "  " + "<color=yellow>Bundle Decompressing Time: </color>" + "<color=white>" + _Timer5.ElapsedMilliseconds + " msec \n</color>";

        _Time1.Stop();
        _AssetInstantceClone = Instantiate(assetLoadRequest.asset, transform) as GameObject;
        _Time1.Stop();
        _Time2.Stop();
        Resources.UnloadUnusedAssets();
        FirstDone = true;

        statsText2 += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Time2.ElapsedMilliseconds);
        statsText += string.Format("\n" + _AssetInstantceClone.name + ": {0} ", _Time1.ElapsedMilliseconds);
        Profiler.EndSample();
    }

    public void Release()
    {


        Profiler.BeginSample("Release");
        Resources.UnloadUnusedAssets();
        _Timer5.Reset();
        _Time1.Reset();
        _Time3.Reset();
        _Time2.Reset();

        AssetBundle.UnloadAllAssetBundles(true);

        DestroyImmediate(_AssetInstantceClone, true);

        AssetBundle.DestroyImmediate(myLoadedAssetBundle, true);
        statsText = string.Format("Placing Asset Time (ms): ");
        statsText2 = string.Format("Complete Loading Asset Time (ms): ");

        Profiler.EndSample();
        System.GC.Collect();

    }

}



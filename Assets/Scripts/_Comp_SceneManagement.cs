using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _Comp_SceneManagement : MonoBehaviour
{

    [SerializeField]
    private GameObject _AllCompression, _AssetbundleRAM, _AssetbundleDisk, _Addressable;
    public Button _Back, _Reload;
    public TMPro.TMP_Text _InfoSelection_Script;


    public void _onClick_Close()
    {
        Addressables.ClearDependencyCacheAsync("default");
        UnityEngine.Caching.ClearCache();
        Caching.ClearCache();
        Resources.UnloadUnusedAssets();
        AsyncOperation operation = SceneManager.UnloadSceneAsync(1, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void _onClick_Reload()
    {
        Addressables.ClearDependencyCacheAsync("default");
        UnityEngine.Caching.ClearCache();
        Caching.ClearCache();
        Resources.UnloadUnusedAssets();
        Scene scene = SceneManager.GetActiveScene();
        AsyncOperation operation = SceneManager.UnloadSceneAsync(1, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        SceneManager.LoadScene(scene.name);
    }

    public static void RestartAndroid()
    {
        if (Application.isEditor) return;

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();

            intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
    }

    public void Awake()
    {

        if (_Comp_SceneManagement_Main.AssetD)
        {
            _AllCompression.SetActive(false);
            _AssetbundleRAM.SetActive(false);
            _AssetbundleDisk.SetActive(true);
            _Addressable.SetActive(false);

        }

        if (_Comp_SceneManagement_Main.AssetR)
        {
            _AllCompression.SetActive(false);
            _AssetbundleRAM.SetActive(true);
            _AssetbundleDisk.SetActive(false);
            _Addressable.SetActive(false);


        }

        if (_Comp_SceneManagement_Main.ADddress1)
        {
            _AllCompression.SetActive(false);
            _AssetbundleRAM.SetActive(false);
            _AssetbundleDisk.SetActive(false);
            _Addressable.SetActive(true);

        }

        if (_Comp_SceneManagement_Main._Comp)
        {
            _AllCompression.SetActive(true);
            _AssetbundleRAM.SetActive(false);
            _AssetbundleDisk.SetActive(false);
            _Addressable.SetActive(false);


        }

    }
    private void Start()
    {
        _InfoSelection_Script.text = _Comp_SceneManagement_Main._scriptname;
    }

}


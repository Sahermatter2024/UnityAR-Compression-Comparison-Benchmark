using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = System.Object;

public class CreatedAssets : MonoBehaviour
{
    [SerializeField] private string _label;
    private List<GameObject> Assets { get; } = new List<GameObject>();

    private void Start()
    {
        // _ = CreateAndWaitUntilComplete();
    }

    public async void CreateAndWaitUntilComplete()
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        //Whatever needs timing here


        // var temp = Time.time;
        //  print("Time for Temp: " + (temp).ToString("f6"));

        await CreateAddressablesLoader.InitByNameOrLabel(_label, Assets);
        //  print("Time for Temp: " + (temp).ToString("f6"));

        foreach (var asset in Assets)
        {
            //Asset is now fully loaded
            UnityEngine.Debug.Log("Loaded asset: " + asset.name);
            asset.transform.parent = this.transform;
            //   print("Time for MyExpensiveFunction: " + (Time.time - temp).ToString("f6"));
            st.Stop();
            UnityEngine.Debug.Log(string.Format("Loading Asset: " + asset.name + "took {0} ms to complete", st.ElapsedMilliseconds));
            //     asset.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }


    }


    public void Release()
    {
        //  await Task.Delay(TimeSpan.FromSeconds(5));
        foreach (var asset in Assets)
        {
            CleanUpFinishedAssets(asset);
        }
    }

    public void CleanUpFinishedAssets(Object obj)
    {
        Addressables.Release(obj);
        UnityEngine.Debug.Log("Unloaded asset: " + obj);
        Resources.UnloadUnusedAssets();
        UnityEngine.Caching.ClearCache(0);


        //return Task.CompletedTask;
    }
}
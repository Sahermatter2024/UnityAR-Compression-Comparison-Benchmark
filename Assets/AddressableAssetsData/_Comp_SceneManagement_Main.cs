using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _Comp_SceneManagement_Main : MonoBehaviour
{
    public static _Comp_SceneManagement_Main instance;

    [SerializeField]
    public TMPro.TMP_Dropdown _DropDownAssetSelection;
    public TMPro.TMP_Dropdown _DropDownScriptSelection;
    public ToggleGroup toggleGroupInstance_Comp;
    public TMPro.TMP_Text _InfoSelection;
    static public bool AssetD, AssetR, ADddress1, _Comp;
    static public string _scriptname;
    List<Texture2D> _textures;
    private void DropdownValueChanged(TMP_Dropdown change)
    {

        if (_DropDownScriptSelection.options[_DropDownScriptSelection.value].text == "Assetbundle on Disk")
        {
            AssetD = true;
            AssetR = false;
            ADddress1 = false;
            _Comp = false;


        }

        if (_DropDownScriptSelection.options[_DropDownScriptSelection.value].text == "Assetbundle on Ram")
        {

            AssetD = false;
            AssetR = true;
            ADddress1 = false;
            _Comp = false;
        }

        if (_DropDownScriptSelection.options[_DropDownScriptSelection.value].text == "Addressable")
        {
            AssetD = false;
            AssetR = false;
            ADddress1 = true;
            _Comp = false;
        }

        if (_DropDownScriptSelection.options[_DropDownScriptSelection.value].text == "All Compressions")
        {
            AssetD = false;
            AssetR = false;
            ADddress1 = false;
            _Comp = true;
        }

    }

    void Start()
    {

        DropdownValueChanged(_DropDownScriptSelection);
        _DropDownScriptSelection.onValueChanged.AddListener(delegate { DropdownValueChanged(_DropDownScriptSelection); });
    }

    public void OnClick()
    {
        _scriptname = _DropDownScriptSelection.options[_DropDownScriptSelection.value].text;
        SceneManager.LoadSceneAsync(1);
    }

    public Toggle currentSelection_Asset
    {
        get { return toggleGroupInstance_Comp.ActiveToggles().FirstOrDefault(); }
    }


}


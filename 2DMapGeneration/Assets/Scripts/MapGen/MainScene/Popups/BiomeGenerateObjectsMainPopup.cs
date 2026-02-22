using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeGenerateObjectsMainPopup : MonoBehaviour
{
    [SerializeField] private GameObject mountainGenButton;
    [SerializeField] private GameObject objectGenButton;
    [SerializeField] private TMP_Dropdown objectTypeChooseDropdown;

    private GameObject uiLoader;
    private PlacableObjectsManager placableObjectsManager;


    private int selectedObjectID;

    void Awake()
    {
        uiLoader = GameObject.FindWithTag("UILoader");
        placableObjectsManager = GameObject.FindWithTag("PlacableObjectsManager").GetComponent<PlacableObjectsManager>();

        PopulateObjectDropdown();
    }

    private void PopulateObjectDropdown()
    {
        objectTypeChooseDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        int currentObjectID = 0;

        while(true)
        {
            GameObject prefab = placableObjectsManager.GetObjectPrefab(currentObjectID);

            if(prefab == null)
            {
                break;
            }

            string displayName = prefab.name;
            Sprite displayPreview = prefab.GetComponent<SpriteRenderer>().sprite;

            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(displayName, displayPreview, Color.white);
            options.Add(option);

            currentObjectID++;
        }

        objectTypeChooseDropdown.AddOptions(options);

        objectTypeChooseDropdown.onValueChanged.RemoveAllListeners();
        objectTypeChooseDropdown.onValueChanged.AddListener(OnDropdownValueChange);

        selectedObjectID = objectTypeChooseDropdown.value;
    }

    private void OnDropdownValueChange(int newValue)
    {
        selectedObjectID = newValue;
    }

    public void SetCallbacks(BiomeBehaviour biomeObject)
    {
        mountainGenButton.GetComponent<Button>().onClick.RemoveAllListeners();

        mountainGenButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            uiLoader.GetComponent<UILoader>().LoadMountainGeneratorPopup(biomeObject);
        });

        objectGenButton.GetComponent<Button>().onClick.RemoveAllListeners();

        objectGenButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            uiLoader.GetComponent<UILoader>().LoadObjectGeneratorPopup(biomeObject, selectedObjectID);
        });
    }
}

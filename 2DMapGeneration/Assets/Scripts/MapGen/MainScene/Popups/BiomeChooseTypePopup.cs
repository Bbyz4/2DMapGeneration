using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeChooseTypePopup : MonoBehaviour
{
    private GameObject biomeTypeManager;
    private GameObject uiLoader;

    private List<GameObject> spawnedButtons;

    [SerializeField] private GameObject typeButtonPrefab;

    void Awake()
    {
        biomeTypeManager = GameObject.FindWithTag("BiomeTypeManager");
        uiLoader = GameObject.FindWithTag("UILoader");

        spawnedButtons = new List<GameObject>();
    }

    private void ClearSpawnedObjects()
    {
        foreach(GameObject b in spawnedButtons)
        {
            Destroy(b);
        }

        spawnedButtons.Clear();
    }

    public void InitializeForGivenBiome(GameObject biomeObject)
    {
        ClearSpawnedObjects();

        int currentBiomeID = 0;

        float currentX = 200f;
        float currentY = 400f;

        while(true)
        {
            BiomeCharacteristics bc = biomeTypeManager.GetComponent<BiomeTypeManager>().GetBiomeCharacteristicsFromID(currentBiomeID);

            if(bc == null)
            {
                break;
            }

            GameObject buttonGO = Instantiate(typeButtonPrefab, transform);

            spawnedButtons.Add(buttonGO);

            RectTransform rect = buttonGO.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(currentX, currentY);

            int capturedID = currentBiomeID;

            Button button = buttonGO.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    uiLoader.GetComponent<UILoader>().DeactivateAllPopups();
                    biomeObject.GetComponent<BiomeBehaviour>().SetBiomeTypeID(capturedID);
                });
            }

            currentX += 200f;
            //currentY -= 100f;

            currentBiomeID++;
        }
    }
}

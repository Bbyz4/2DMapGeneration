using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void InitializeForGivenBiome(BiomeBehaviour biomeBeh)
    {
        GameObject biomeObject = biomeBeh.gameObject;

        ClearSpawnedObjects();

        int currentBiomeID = 0;

        float currentX = -567f;
        float currentY = 330f;

        while(true)
        {
            BiomeCharacteristics bc = biomeTypeManager.GetComponent<BiomeTypeManager>().GetBiomeCharacteristicsFromID(currentBiomeID);

            if(bc == null)
            {
                break;
            }

            GameObject buttonGO = Instantiate(typeButtonPrefab, transform);

            buttonGO.transform.GetChild(0).GetComponent<TMP_Text>().text = bc.BiomeName;

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

            currentX += 567f;

            if(currentX > 1000f)
            {
                currentX = -567f;
                currentY -= 90f;
            }

            currentBiomeID++;
        }
    }
}

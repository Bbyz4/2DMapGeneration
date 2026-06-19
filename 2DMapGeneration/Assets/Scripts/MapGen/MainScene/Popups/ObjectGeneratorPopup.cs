using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGeneratorPopup : MonoBehaviour
{
    private ArgumentCollector argumentCollector;

    void Awake()
    {
        argumentCollector = GameObject.FindWithTag("ArgumentCollector").GetComponent<ArgumentCollector>();
    }

    private Dictionary<string,int> algNameToID = new Dictionary<string, int>()
    {
        {"Poisson sampling", 0}
    };

    public void InitializeForGivenBiome(BiomeBehaviour biomeBeh, int generatedObjectID)
    {
        foreach(Transform child in transform)
        {
            Button button = child.GetComponent<Button>();
            if(button == null)
            {
                continue;
            }

            TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>();
            if(tmpText == null)
            {
                continue;
            }

            if (algNameToID.ContainsKey(tmpText.text))
            {
                button.onClick.RemoveAllListeners();

                int capturedValue = algNameToID[tmpText.text];

                button.onClick.AddListener(() =>
                {
                    argumentCollector.CollectAndLaunchObjectArgs(capturedValue, biomeBeh, generatedObjectID);
                });
            }
        }
    }
}

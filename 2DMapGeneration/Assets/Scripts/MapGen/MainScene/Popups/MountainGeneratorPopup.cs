using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MountainGeneratorPopup : MonoBehaviour
{
    private ArgumentCollector argumentCollector;

    void Awake()
    {
        argumentCollector = GameObject.FindWithTag("ArgumentCollector").GetComponent<ArgumentCollector>();
    }

    private Dictionary<string,int> algNameToID = new Dictionary<string, int>()
    {
        {"Perlin Noise", 0},
        {"WFC", 1},
        {"Weierstrass", 2},
        {"Simplex Noise", 3}  
    };
    public void InitializeForGivenBiome(BiomeBehaviour biomeBeh)
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
                    argumentCollector.CollectAndLaunchMountainArgs(capturedValue, biomeBeh);
                });
            }
        }
    }
}

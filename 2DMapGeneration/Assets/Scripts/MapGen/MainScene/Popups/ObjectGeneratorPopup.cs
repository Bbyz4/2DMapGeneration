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

            if (int.TryParse(tmpText.text, out int parsedValue))
            {
                button.onClick.RemoveAllListeners();

                int capturedValue = parsedValue;

                button.onClick.AddListener(() =>
                {
                    argumentCollector.CollectAndLaunchObjectArgs(capturedValue, biomeBeh, generatedObjectID);
                });
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BiomeGenerateObjectsMainPopup : MonoBehaviour
{
    [SerializeField] private GameObject mountainGenButton;
    [SerializeField] private GameObject objectGenButton;

    private GameObject uiLoader;

    void Awake()
    {
        uiLoader = GameObject.FindWithTag("UILoader");
    }

    public void SetCallbacks(GameObject biomeObject)
    {
        mountainGenButton.GetComponent<Button>().onClick.RemoveAllListeners();

        mountainGenButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            uiLoader.GetComponent<UILoader>().LoadMountainGeneratorPopup(biomeObject);
        });

        objectGenButton.GetComponent<Button>().onClick.RemoveAllListeners();

        objectGenButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            uiLoader.GetComponent<UILoader>().LoadObjectGeneratorPopup(biomeObject);
        });
    }
}

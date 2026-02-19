using UnityEngine;

public class UILoader : MonoBehaviour
{
    [SerializeField] private BiomeGeneratorPopup biomeGeneratorPopup;
    [SerializeField] private BiomeChooseTypePopup biomeChooseTypePopup;
    [SerializeField] private MountainGeneratorPopup mountainGeneratorPopup;
    [SerializeField] private ObjectGeneratorPopup objectGeneratorPopup;

    void Awake()
    {
        DeactivateAllPopups();
        biomeGeneratorPopup.gameObject.SetActive(true);
    }

    public void DeactivateAllPopups()
    {
        biomeGeneratorPopup.gameObject.SetActive(false);
        biomeChooseTypePopup.gameObject.SetActive(false);
        mountainGeneratorPopup.gameObject.SetActive(false);
        objectGeneratorPopup.gameObject.SetActive(false);
    }

    public void LoadBiomeChooseTypePopup(GameObject biomeObject)
    {
        DeactivateAllPopups();

        biomeChooseTypePopup.gameObject.SetActive(true);

        biomeChooseTypePopup.GetComponent<BiomeChooseTypePopup>().InitializeForGivenBiome(biomeObject);
    }

    public void LoadBiomeGenerateObjectsMainPopup(GameObject biomeObject)
    {
        DeactivateAllPopups();

        //to be implemented
    }

    public void HideBiomeGeneratorPopup()
    {
        biomeGeneratorPopup.gameObject.SetActive(false);
    }
}

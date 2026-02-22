using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UILoader : MonoBehaviour
{
    [SerializeField] private BiomeGeneratorPopup biomeGeneratorPopup;
    [SerializeField] private BiomeChooseTypePopup biomeChooseTypePopup;
    [SerializeField] private BiomeGenerateObjectsMainPopup biomeGenerateObjectsMainPopup;
    [SerializeField] private MountainGeneratorPopup mountainGeneratorPopup;
    [SerializeField] private ObjectGeneratorPopup objectGeneratorPopup;

    //make those enums, this is getting out of hand...

    void Awake()
    {
        DeactivateAllPopups();
        biomeGeneratorPopup.gameObject.SetActive(true);
    }

    public void DeactivateAllPopups()
    {
        biomeGeneratorPopup.gameObject.SetActive(false);
        biomeChooseTypePopup.gameObject.SetActive(false);
        biomeGenerateObjectsMainPopup.gameObject.SetActive(false);
        mountainGeneratorPopup.gameObject.SetActive(false);
        objectGeneratorPopup.gameObject.SetActive(false);
    }

    public void LoadBiomeChooseTypePopup(BiomeBehaviour biomeObject)
    {
        DeactivateAllPopups();

        biomeChooseTypePopup.gameObject.SetActive(true);

        biomeChooseTypePopup.GetComponent<BiomeChooseTypePopup>().InitializeForGivenBiome(biomeObject);
    }

    public void LoadBiomeGenerateObjectsMainPopup(BiomeBehaviour biomeObject)
    {
        DeactivateAllPopups();

        biomeGenerateObjectsMainPopup.gameObject.SetActive(true);

        biomeGenerateObjectsMainPopup.SetCallbacks(biomeObject);
    }

    public void LoadMountainGeneratorPopup(BiomeBehaviour biomeObject)
    {
        DeactivateAllPopups();

        mountainGeneratorPopup.gameObject.SetActive(true);

        mountainGeneratorPopup.InitializeForGivenBiome(biomeObject);
    }

    public void LoadObjectGeneratorPopup(BiomeBehaviour biomeObject, int objectID)
    {
        DeactivateAllPopups();

        objectGeneratorPopup.gameObject.SetActive(true);

        objectGeneratorPopup.InitializeForGivenBiome(biomeObject);
    }

    public void HideBiomeGeneratorPopup()
    {
        biomeGeneratorPopup.gameObject.SetActive(false);
    }

    public void HideMountainGeneratorPopup()
    {
        mountainGeneratorPopup.gameObject.SetActive(false);
    }

    public void HideObjectGeneratorPopup()
    {
        objectGeneratorPopup.gameObject.SetActive(false);
    }
}

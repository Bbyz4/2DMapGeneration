using UnityEngine;
using UnityEngine.EventSystems;

public class BiomeBehaviour : MonoBehaviour, IPointerDownHandler
{
    private GameObject UILoader;
    private GameObject biomeTypeManager;
    private int biomeTypeID;
    private BiomeCharacteristics biomeChars;
    void Awake()
    {
        UILoader = GameObject.FindWithTag("UILoader");
        biomeTypeManager = GameObject.FindWithTag("BiomeTypeManager");

        biomeTypeID = -1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(biomeTypeID == -1)
        {
            UILoader.GetComponent<UILoader>().LoadBiomeChooseTypePopup(gameObject);
        }
        else
        {
            UILoader.GetComponent<UILoader>().LoadBiomeGenerateObjectsMainPopup(gameObject);
        }    
    }

    public int GetBiomeTypeID()
    {
        return biomeTypeID;
    }

    public void SetBiomeTypeID(int newID)
    {
        BiomeCharacteristics newBiomeChars = biomeTypeManager.GetComponent<BiomeTypeManager>().GetBiomeCharacteristicsFromID(newID);

        if(newBiomeChars != null)
        {
            biomeTypeID = newID;   
            biomeChars = newBiomeChars;

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material.mainTexture = biomeChars.GetDefaultTurf();

        }
    }
}

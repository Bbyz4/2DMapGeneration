using UnityEngine;

public class BiomeBehaviour : MonoBehaviour
{
    private GameObject UILoader;
    private GameObject biomeTypeManager;
    private int biomeTypeID;
    private BiomeCharacteristics biomeChars;
    void Awake()
    {
        UILoader = GameObject.FindWithTag("UILoader");
        biomeTypeManager = GameObject.FindWithTag("BiomeTypeManager");

        gameObject.AddComponent<PolygonCollider2D>();

        biomeTypeID = -1;
    }

    void OnMouseDown()
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
        }
    }
}

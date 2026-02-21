using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BiomeBehaviour : MonoBehaviour, IPointerDownHandler
{

    private BiomeData myData;
    private List<MountainData> mountainDatas;
    private List<GameObject> mountainObjects;
    private List<ObjectData> objectDatas;
    private List<GameObject> objectObjects;

    private GameObject UILoader;
    private GameObject biomeTypeManager;
    private int biomeTypeID;
    private BiomeCharacteristics biomeChars;
    void Awake()
    {
        UILoader = GameObject.FindWithTag("UILoader");
        biomeTypeManager = GameObject.FindWithTag("BiomeTypeManager");

        biomeTypeID = -1;

        mountainDatas = new List<MountainData>();
        mountainObjects = new List<GameObject>();
        objectDatas = new List<ObjectData>();
        objectObjects = new List<GameObject>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(biomeTypeID == -1)
        {
            UILoader.GetComponent<UILoader>().LoadBiomeChooseTypePopup(this);
        }
        else
        {
            UILoader.GetComponent<UILoader>().LoadBiomeGenerateObjectsMainPopup(this);
        }    
    }

    public void SetBiomeData(BiomeData data)
    {
        myData = data;
    }

    public BiomeData GetData()
    {
        return myData;
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

    public void AddMountain(MountainData data, GameObject mountainObject)
    {
        mountainDatas.Add(data);
        mountainObjects.Add(mountainObject);
    }

    public void AddObject(ObjectData data, GameObject objectObject)
    {
        objectDatas.Add(data);
        objectObjects.Add(objectObject);
    }

    public IReadOnlyList<MountainData> GetMountainDatas()
    {
        return mountainDatas;
    }

    public IReadOnlyList<ObjectData> GetObjectDatas()
    {
        return objectDatas;
    }

    public BiomeCharacteristics GetCharacteristics()
    {
        return biomeChars;
    }
}

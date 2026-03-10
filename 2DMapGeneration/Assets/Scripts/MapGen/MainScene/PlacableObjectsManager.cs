using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectCharacteristics
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private bool isPlacableOnLake;
    [SerializeField] private bool isPlacableOnSurface;
    [SerializeField] private bool isPlacableOnMountainLevel1;
    [SerializeField] private bool isPlacableOnMountainLevel2;
    [SerializeField] private bool isPlacableOnMountainLevel3;

    public GameObject GetObjectPrefab()
    {
        return objectPrefab;
    }

    public bool IsPlacableOnGivenLevel(int level)
    {
        switch(level)
        {
            case -1:
                return isPlacableOnLake;
            case 0:
                return isPlacableOnSurface;
            case 1:
                return isPlacableOnMountainLevel1;
            case 2:
                return isPlacableOnMountainLevel2;
            case 3:
                return isPlacableOnMountainLevel3;
            default:
                return false;
        }
    }
}

public class PlacableObjectsManager : MonoBehaviour
{
    [SerializeField] private List<ObjectCharacteristics> objects;

    public GameObject GetObjectPrefab(int objectID)
    {
        if(objectID < 0 || objectID >= objects.Count)
        {
            return null;
        }

        return objects[objectID].GetObjectPrefab();
    }

    public Dictionary<int, bool> GetObjectPlacabilityDict(int objectID)
    {
        if(objectID < 0 || objectID >= objects.Count)
        {
            return null;
        }

        Dictionary<int, bool> result = new Dictionary<int, bool>();

        for(int h=-1; h<=3; h++)
        {
            result[h] = objects[objectID].IsPlacableOnGivenLevel(h);
        }

        return result;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlacableObjectsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectPrefabs;

    public GameObject GetObjectPrefab(int objectID)
    {
        if(objectID < 0 || objectID >= objectPrefabs.Count)
        {
            return null;
        }

        return objectPrefabs[objectID];
    }
}

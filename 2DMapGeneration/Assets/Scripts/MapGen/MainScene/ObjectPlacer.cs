using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private MapJSONBuilder mapJSONBuilder;

    private PlacableObjectsManager placableObjectsManager;

    [SerializeField] private Texture2D defaultBiomeTexture;

    public Vector2Int MAP_SIZE {get; private set;} = new Vector2Int(500,500); //later move to a separate class with constants or prompt the user at start

    void Awake()
    {
        mapJSONBuilder = new MapJSONBuilder(MAP_SIZE);

        placableObjectsManager = GameObject.FindWithTag("PlacableObjectsManager").GetComponent<PlacableObjectsManager>();
    }

    public void PlaceBiomes(List<BiomeData> biomeList)
    {
        foreach(BiomeData biome in biomeList)
        {
            GameObject newBiome = OutlineUtils.CreateShapeObject($"Biome", biome.outline, defaultBiomeTexture, this.transform, Color.black, 0.1f, 0f);
            mapJSONBuilder.AddBiome(biome);

            newBiome.AddComponent<BiomeBehaviour>();
            newBiome.GetComponent<BiomeBehaviour>().SetBiomeData(biome);

            //do something else, for instance set sorting layer, ordering layer etc.
        }

        SaveJSON();
    }

    public void PlaceMountains(List<MountainData> mountainList, BiomeBehaviour parentBiome)
    {
        foreach(MountainData mountain in mountainList)
        {
            float zOrder = mountain.elevationLevel == -1 ? -1f : mountain.elevationLevel * -1f;

            GameObject newMountain = OutlineUtils.CreateShapeObject($"Mountain", mountain.outline, parentBiome.GetCharacteristics().GetMountainTurf(mountain.elevationLevel), parentBiome.transform, Color.black, 0.1f, zOrder);
            mapJSONBuilder.AddMountain(mountain);

            parentBiome.AddMountain(mountain, newMountain);
        }
    }

    public void PlaceObjects(List<ObjectData> objectList, BiomeBehaviour parentBiome)
    {
        foreach(ObjectData obj in objectList)
        {
            GameObject newObj = Instantiate(placableObjectsManager.GetObjectPrefab(obj.objectID), new Vector3(obj.position.x, obj.position.y, 0f), Quaternion.identity, parentBiome.transform);
            mapJSONBuilder.AddObject(obj);

            parentBiome.AddObject(obj, newObj);
        }
    }

    private void SaveJSON()
    {
        mapJSONBuilder.SaveToJson("Assets/Scripts/MapGen/Tests/test_map.json");
    }
}

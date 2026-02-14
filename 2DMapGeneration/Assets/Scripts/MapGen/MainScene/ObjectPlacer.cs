using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private MapJSONBuilder mapJSONBuilder;

    [SerializeField] private Texture2D defaultBiomeTexture;

    public Vector2Int MAP_SIZE {get; private set;} = new Vector2Int(500,500); //later move to a separate class with constants or prompt the user at start

    void Awake()
    {
        mapJSONBuilder = new MapJSONBuilder(MAP_SIZE);
    }

    public void PlaceBiomes(List<BiomeData> biomeList)
    {
        foreach(BiomeData biome in biomeList)
        {
            GameObject newBiome = OutlineUtils.CreateShapeObject($"Biome", biome.outline, defaultBiomeTexture, this.transform, Color.black, 0.1f);
            mapJSONBuilder.AddBiome(biome);

            newBiome.AddComponent<BiomeBehaviour>();

            //do something else, for instance set sorting layer, ordering layer etc.
        }

        SaveJSON();
    }

    private void SaveJSON()
    {
        mapJSONBuilder.SaveToJson("Assets/Scripts/MapGen/Tests/test_map.json");
    }
}

using UnityEngine;
using System.IO;

public class MapJSONBuilder
{
    private MapData mapData;

    public MapJSONBuilder(Vector2 mapSize)
    {
        mapData = new MapData
        {
            mapSize = mapSize
        };
    }

    public void AddBiome(BiomeData biome)
    {
        mapData.biomes.Add(biome);
    }

    public void AddMountain(MountainData mountain)
    {
        mapData.mountains.Add(mountain);
    }

    public void AddObject(ObjectData obj)
    {
        mapData.objects.Add(obj);
    }

    public MapData GetMapData()
    {
        return mapData;
    }

    public void SaveToJson(string path)
    {
        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(path, json);
    }

    public static MapData LoadFromJson(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<MapData>(json);
    }
}

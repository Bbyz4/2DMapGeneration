using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public Vector2 mapSize;

    public List<BiomeData> biomes = new();
    public List<MountainData> mountains = new();
    public List<ObjectData> objects = new();
}
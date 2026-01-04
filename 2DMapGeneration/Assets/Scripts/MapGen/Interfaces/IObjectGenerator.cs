using System.Collections.Generic;
using UnityEngine;

public interface IObjectGenerator
{
    public List<ObjectData> Generate(BiomeData biome, List<MountainData> mountains);
}

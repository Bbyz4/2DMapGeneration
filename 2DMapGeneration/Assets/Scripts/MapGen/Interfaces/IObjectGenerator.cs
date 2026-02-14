using System.Collections.Generic;
using UnityEngine;

public interface IObjectGenerator
{
    public void Initialize(IObjectGeneratorArgs args);
    public List<ObjectData> Generate(BiomeData biome, List<MountainData> mountains);
}

using System.Collections.Generic;
using UnityEngine;

public interface IMountainGenerator
{
    public void Initialize(IMountainGeneratorArgs args);
    public List<MountainData> Generate(BiomeData biome);
}

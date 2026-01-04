using System.Collections.Generic;
using UnityEngine;

public interface IMountainGenerator
{
    public List<MountainData> Generate(BiomeData biome);
}

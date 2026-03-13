using System.Collections.Generic;
using UnityEngine;

public class MountainGeneratorResult
{
    public int[,] elevationMap;
    public int startX;
    public int startY;
}

public interface IMountainGenerator
{
    public void Initialize(IMountainGeneratorArgs args);
    public MountainGeneratorResult Generate(BiomeData biome);
}

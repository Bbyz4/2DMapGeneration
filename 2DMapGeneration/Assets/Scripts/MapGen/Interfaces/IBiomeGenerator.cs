using System.Collections.Generic;
using UnityEngine;

public interface IBiomeGenerator
{
    public void Initialize(IBiomeGeneratorArgs args);
    public List<BiomeData> Generate(Vector2 mapSize);
}

using System.Collections.Generic;
using UnityEngine;

public interface IBiomeGenerator
{
    public List<BiomeData> Generate(Vector2 mapSize);
}

using System;

[Serializable]
public class VoronoiIslandBiomeGeneratorArgs : IBiomeGeneratorArgs
{
    public int targetFinalBiomeAmount;
    public int startingClusterAmount;
    public bool excludeEdgeClusters;
}
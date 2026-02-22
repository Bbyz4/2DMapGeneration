using System;

[Serializable]
public class PoissonObjectGeneratorArgs : IObjectGeneratorArgs
{
    public float minDistance;
    public int maxAttempts;
    public int maxObjects;
    public int initialClusters;
}
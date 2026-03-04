using System;

[Serializable]
public class WFCMountainGeneratorArgs : IMountainGeneratorArgs
{
    public float lakeProbability;
    public float surfaceProbability;
    public float mountain1Probability;
    public float mountain2Probability;
    public float mountain3Probability;
    public float clusteringCoefficient;
}
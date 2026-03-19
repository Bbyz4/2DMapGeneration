using System;

[Serializable]
public class SimplexMountainGeneratorArgs : IMountainGeneratorArgs
{
    public float scale;
    public float a;
    public float b;
    public float c;
    public float d;
    public int octaves;
    public float amplitudeMultiplier;
    public float frequencyMultiplier;
}
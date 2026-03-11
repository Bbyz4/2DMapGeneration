using System;

[Serializable]
public class WeierstrassMountainGeneratorArgs : IMountainGeneratorArgs
{
    public float D;
    public float G;
    public float gamma;
    public int M;
    public int n_max;
    public float scale;
}
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
    public float a;
    public float b;
    public float c;
    public float d;
}
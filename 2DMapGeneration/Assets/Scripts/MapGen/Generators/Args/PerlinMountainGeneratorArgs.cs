using System;

[Serializable]
public class PerlinMountainGeneratorArgs : IMountainGeneratorArgs
{
    public float scale;
    public float a;
    public float b;
    public float c;
    public float d;
}
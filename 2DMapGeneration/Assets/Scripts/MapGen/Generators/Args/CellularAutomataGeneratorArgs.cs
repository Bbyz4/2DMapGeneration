using System;

[Serializable]
public class CellularAutomataGeneratorArgs : IMountainGeneratorArgs
{
    public float elevationProbability;
    public bool enforceSmoothedEdges;
    public int automataIterations;
}
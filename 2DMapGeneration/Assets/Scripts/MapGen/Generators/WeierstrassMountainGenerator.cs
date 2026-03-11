using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class WeierstrassMountainGenerator : MonoBehaviour, IMountainGenerator
{
    private WeierstrassMountainGeneratorArgs args;

    private System.Random rand = new System.Random();
    private float[,] phi;


    public void Initialize(IMountainGeneratorArgs args)
    {
        this.args = (WeierstrassMountainGeneratorArgs)args;

        phi = new float[this.args.M, this.args.n_max];

        for(int m=0; m<this.args.M; m++)
        {
            for(int n=0; n<this.args.n_max; n++)
            {
                phi[m,n] = (float)rand.NextDouble() * 2 * MathF.PI;
            }
        }
    }

    public float GetWeierstrassMandelbrotValue(float x, float y, float L)
    {
        float A = MathF.Pow(L * (args.G/L), args.D-2) * MathF.Pow(MathF.Log(args.gamma)/args.M, 0.5f);  
        float z = 0;
        float r = MathF.Sqrt(x*x + y*y);

        for(int m=1; m<=args.M; m++)
        {
            float theta_m = MathF.Atan2(y, x) - (MathF.PI * m / args.M);

            for(int n=1; n<=args.n_max; n++)
            {
                float gamma_n = MathF.Pow(args.gamma, n);
                float phi_mn = phi[m-1,n-1];
                float term = MathF.Cos(phi_mn) - MathF.Cos((2 * MathF.PI * gamma_n * r * MathF.Cos(theta_m) / L) + phi_mn);
                z += MathF.Pow(args.gamma, ((args.D - 3) * n)) * term;
            }
        }

        return A*z;
    }

    public List<MountainData> Generate(BiomeData biome)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        int width = (int)(bounds.xMax - bounds.xMin);
        int height = (int)(bounds.yMax - bounds.yMin);

        int[,] elevationMap = new int[width, height];
        float[,] computedValues = new float[width, height];

        float minComputed = float.MaxValue;
        float maxComputed = float.MinValue;

        float L = MathF.Max(width, height);

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                computedValues[x,y] = GetWeierstrassMandelbrotValue(x - 0.5f*width, y - 0.5f*height, L*args.scale);
                minComputed = MathF.Min(minComputed, computedValues[x,y]);
                maxComputed = MathF.Max(maxComputed, computedValues[x,y]);
            }
        }

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                float normalized = (computedValues[x,y]-minComputed)/(maxComputed-minComputed);
                elevationMap[x,y] = (int)(-2 + 6*normalized);
            }
        }

        List<MountainData> result = GeneratorUtils.BuildMountainOutlines(elevationMap, width, height, (int)bounds.xMin, (int)bounds.yMin);
    
        List<MountainData> filtered = new List<MountainData>();

        foreach (MountainData md in result)
        {
            List<List<Vector2>> clipped = OutlineUtils.GetOutlinesIntersection(md.outline, biome.outline);

            if (clipped != null)
            {
                foreach(List<Vector2> newOutline in clipped)
                {
                    MountainData newMD = new MountainData();
                    newMD.outline = newOutline;
                    newMD.elevationLevel = md.elevationLevel;
                    filtered.Add(newMD);
                }
            }
        }

        return filtered;
    }
}
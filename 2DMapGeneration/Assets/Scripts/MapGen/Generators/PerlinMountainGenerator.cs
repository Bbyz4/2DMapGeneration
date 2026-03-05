using UnityEngine;
using System.Collections.Generic;
using System;

public class PerlinMountainGenerator : MonoBehaviour, IMountainGenerator
{
    private PerlinMountainGeneratorArgs args;
    private PerlinNoise noise;

    private int Classify(float h)
    {
        if(h <= args.a) return -1; 
        if(h <= args.b) return 0;  
        if(h <= args.c) return 1;  
        if(h <= args.d) return 2; 
        return 3;             
    }

    public void Initialize(IMountainGeneratorArgs args)
    {
        this.args = (PerlinMountainGeneratorArgs)args;
    }

    public List<MountainData> Generate(BiomeData biome)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        int width = (int)(bounds.xMax - bounds.xMin);
        int height = (int)(bounds.yMax - bounds.yMin);

        noise = new PerlinNoise(0, width-1, height-1, 0);
        noise.Randomize();

        int[,] elevationMap = new int[width, height];

        // 1. Sample noise → elevation map
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float h = noise.Sample(x * args.scale, y * args.scale);
                elevationMap[x, y] = Classify(h);
            }
        }

        List<MountainData> result = GeneratorUtils.BuildMountainOutlines(elevationMap, width, height, (int)bounds.xMin, (int)bounds.yMin);

        List<Vector2> biomeOutlineNoBorder = OutlineUtils.CutOutlineBorder(biome.outline, 2);

        if(biomeOutlineNoBorder == null)
        {
            Debug.Log("A");
        }

        List<MountainData> filtered = new List<MountainData>();

        foreach (MountainData md in result)
        {
            List<List<Vector2>> clipped = OutlineUtils.GetOutlinesIntersection(md.outline, biomeOutlineNoBorder);

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
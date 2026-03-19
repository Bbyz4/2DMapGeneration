using UnityEngine;
using System.Collections.Generic;
using System;

public class SimplexMountainGenerator : MonoBehaviour, IMountainGenerator
{
    private SimplexMountainGeneratorArgs args;
    private SimplexNoise noise;

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
        this.args = (SimplexMountainGeneratorArgs)args;
    }

    public MountainGeneratorResult Generate(BiomeData biome)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        int width = (int)(bounds.xMax - bounds.xMin);
        int height = (int)(bounds.yMax - bounds.yMin);

        noise = new SimplexNoise(70f);

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

        MountainGeneratorResult mgr = new MountainGeneratorResult
        {
            elevationMap = elevationMap,
            startX = Mathf.FloorToInt(bounds.xMin),
            startY = Mathf.FloorToInt(bounds.yMin),
        };

        return mgr;
    }
}
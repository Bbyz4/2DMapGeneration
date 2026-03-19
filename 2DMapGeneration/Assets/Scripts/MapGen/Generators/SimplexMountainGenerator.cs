using UnityEngine;
using System.Collections.Generic;
using System;

public class SimplexMountainGenerator : MonoBehaviour, IMountainGenerator
{
    private SimplexMountainGeneratorArgs args;

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

        List<SimplexNoise> noiseList = new List<SimplexNoise>();

        for(int i=0; i<args.octaves; i++)
        {
            int randomSeed = UnityEngine.Random.Range(int.MinValue+1, int.MaxValue-1);
            noiseList.Add(new SimplexNoise(randomSeed, 70f));
        }

        int[,] elevationMap = new int[width, height];

        // 1. Sample noise → elevation map
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float total = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float maxValue = 0f;

                for(int i=0; i<args.octaves; i++)
                {
                    float h = noiseList[i].Sample(x * args.scale * frequency, y * args.scale * frequency);

                    total += h * amplitude;
                    maxValue += amplitude;

                    frequency *= args.frequencyMultiplier;
                    amplitude *= args.amplitudeMultiplier;
                }

                elevationMap[x, y] = Classify(total/maxValue);
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
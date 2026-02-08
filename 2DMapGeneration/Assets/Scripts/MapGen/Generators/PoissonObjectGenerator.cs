using UnityEngine;
using System.Collections.Generic;

public class PoissonObjectGenerator : IObjectGenerator
{
    private int width;
    private int height;
    private float minDistance;
    private int maxAttempts;
    private int maxObjects;
    private int objectID;
    private int initialClusters;

    private PoissonSampling sampler;

    public PoissonObjectGenerator(
        int width,
        int height,
        float minDistance,
        int maxAttempts,
        int maxObjects,
        int initialClusters,
        int objectID)
    {
        this.width = width;
        this.height = height;
        this.minDistance = minDistance;
        this.maxAttempts = maxAttempts;
        this.maxObjects = maxObjects;
        this.objectID = objectID;
        this.initialClusters = initialClusters;

        sampler = new PoissonSampling(
            left: 0f,
            right: width,
            bottom: 0f,
            top: height,
            minDistance: minDistance,
            maxPoints: maxObjects,
            initialPoints: initialClusters,
            rejectionSamples: maxAttempts
        );
    }

    public List<ObjectData> Generate(BiomeData biome, List<MountainData> mountains)
    {
        List<Vector2> points = sampler.Generate();
        List<ObjectData> result = new List<ObjectData>(points.Count);

        foreach (var p in points)
        {
            result.Add(new ObjectData
            {
                position = p,
                objectID = objectID
            });
        }

        return result;
    }
}

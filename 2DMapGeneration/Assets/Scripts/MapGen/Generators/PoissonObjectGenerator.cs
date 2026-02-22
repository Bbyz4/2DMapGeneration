using UnityEngine;
using System.Collections.Generic;

public class PoissonObjectGenerator : MonoBehaviour, IObjectGenerator
{

    private PoissonObjectGeneratorArgs args;
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

    public void Initialize(IObjectGeneratorArgs args)
    {
        this.args = (PoissonObjectGeneratorArgs)args;
    }

    public List<ObjectData> Generate(BiomeData biome, List<MountainData> mountains, int generatedObjectID)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        sampler = new PoissonSampling(
            left: bounds.xMin,
            right: bounds.xMax,
            bottom: bounds.yMin,
            top: bounds.yMax,
            minDistance: args.minDistance,
            maxPoints: args.maxObjects,
            initialPoints: args.initialClusters,
            rejectionSamples: args.maxAttempts
        );

        List<Vector2> points = sampler.Generate();
        List<ObjectData> result = new List<ObjectData>();

        foreach (var p in points)
        {
            if(OutlineUtils.IsPointInOutline(p, biome.outline))
            {
                result.Add(new ObjectData
                {
                    position = p,
                    objectID = generatedObjectID
                });
            }
        }

        return result;
    }
}

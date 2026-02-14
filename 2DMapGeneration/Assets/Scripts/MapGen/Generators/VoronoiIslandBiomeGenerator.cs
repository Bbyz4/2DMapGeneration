using UnityEngine;
using System.Collections.Generic;

public class VoronoiIslandBiomeGenerator : MonoBehaviour, IBiomeGenerator
{

    private int width;
    private int height;
    private int targetFinalBiomeAmount;
    private int startingClusterAmount;
    private bool excludeEdgeClusters;

    //---

    private VoronoiDiagram diagram;

    public VoronoiIslandBiomeGenerator(
        int width,
        int height,
        int targetFinalBiomeAmount,
        int startingClusterAmount,
        bool excludeEdgeClusters
    )
    {
        this.width = width;
        this.height = height;
        this.targetFinalBiomeAmount = targetFinalBiomeAmount;
        this.startingClusterAmount = startingClusterAmount;
        this.excludeEdgeClusters = excludeEdgeClusters;

        diagram = new VoronoiDiagram(0, width-1, height-1, 0);
    }

    public List<BiomeData> Generate(Vector2 mapSize)
    {
        List<BiomeData> result = new List<BiomeData>();

        diagram.Generate(startingClusterAmount, (Vector2 a, Vector2 b) => {return Vector2.Distance(a,b);});

        int[,] biomeMap = new int[width, height];

        //remove biomes that touch the edge of the map if the option was selected
        HashSet<int> removedBiomes = new HashSet<int>();

        if(excludeEdgeClusters)
        {
            for(int i=0; i<height; i++)
            {
                int biomeIndex = diagram.GetRegionIndex(0, i);
                removedBiomes.Add(biomeIndex);

                int biomeIndex2 = diagram.GetRegionIndex(width-1, i);
                removedBiomes.Add(biomeIndex2);
            }

            for(int i=0; i<width; i++)
            {
                int biomeIndex = diagram.GetRegionIndex(i, 0);
                removedBiomes.Add(biomeIndex);

                int biomeIndex2 = diagram.GetRegionIndex(i, height-1);
                removedBiomes.Add(biomeIndex2);
            }
        }

        //hierarchical clustering on remaining biomes

        List<Vector2Int> connectingQueue = new List<Vector2Int>();

        List<Vector2> sites = (List<Vector2>)diagram.GetSites();

        for(int i=0; i<sites.Count; i++)
        {
            for(int j=i+1; j<sites.Count; j++)
            {
                if(i!=j && !removedBiomes.Contains(diagram.GetRegionIndex(sites[i].x, sites[i].y)) && !removedBiomes.Contains(diagram.GetRegionIndex(sites[j].x, sites[j].y)))
                {
                    connectingQueue.Add(new Vector2Int(i,j));
                }
            }
        }

        //sort the queue based on distance
        connectingQueue.Sort((a, b) =>
        {
            float distA = Vector2.SqrMagnitude(sites[a.x] - sites[a.y]);
            float distB = Vector2.SqrMagnitude(sites[b.x] - sites[b.y]);
            return distA.CompareTo(distB);
        });


        DSU dsu = new DSU(startingClusterAmount);

        int currentActiveClusters = startingClusterAmount - removedBiomes.Count;

        int currentLookedQueueIndex = 0;

        while(currentActiveClusters > targetFinalBiomeAmount && currentLookedQueueIndex < connectingQueue.Count)
        {
            Vector2Int potentialConnection = connectingQueue[currentLookedQueueIndex];
            currentLookedQueueIndex++;

            if(!dsu.Connected(potentialConnection.x, potentialConnection.y))
            {
                currentActiveClusters--;

                dsu.Union(potentialConnection.x, potentialConnection.y);
            }
        }

        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                int biomeIndex = diagram.GetRegionIndex(i,j);

                biomeMap[i,j] = removedBiomes.Contains(biomeIndex) ? -1 : dsu.Find(biomeIndex);
            }
        }

        result = GeneratorUtils.BuildBiomeOutlines(biomeMap, width, height);

        return result;
    }
}
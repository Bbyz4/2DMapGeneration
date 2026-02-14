using UnityEngine;
using System.Collections.Generic;

public class VoronoiIslandBiomeGenerator : MonoBehaviour, IBiomeGenerator
{

    private VoronoiIslandBiomeGeneratorArgs args;

    private VoronoiDiagram diagram;

    public void Initialize(IBiomeGeneratorArgs args)
    {
        this.args = (VoronoiIslandBiomeGeneratorArgs)args;
    }

    public List<BiomeData> Generate(Vector2 mapSize)
    {
        int width = (int)mapSize.x;
        int height = (int)mapSize.y;
        

        List<BiomeData> result = new List<BiomeData>();

        diagram.Generate(args.startingClusterAmount, (Vector2 a, Vector2 b) => {return Vector2.Distance(a,b);});

        int[,] biomeMap = new int[width, height];

        //remove biomes that touch the edge of the map if the option was selected
        HashSet<int> removedBiomes = new HashSet<int>();

        if(args.excludeEdgeClusters)
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


        DSU dsu = new DSU(args.startingClusterAmount);

        int currentActiveClusters = args.startingClusterAmount - removedBiomes.Count;

        int currentLookedQueueIndex = 0;

        while(currentActiveClusters > args.targetFinalBiomeAmount && currentLookedQueueIndex < connectingQueue.Count)
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
using System.Collections.Generic;
using UnityEngine;

class Edge
{
    public int a;
    public int b;
    public float dist;

    public Edge(int a, int b, float dist)
    {
        this.a = a;
        this.b = b;
        this.dist = dist;
    }
}

class UnionFind
{
    private int[] parent;
    private int[] rank;
    public int SetCount { get; private set; }

    public UnionFind(int size)
    {
        parent = new int[size];
        rank = new int[size];
        SetCount = size;

        for (int i = 0; i < size; i++)
            parent[i] = i;
    }

    public int Find(int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent[x]);
        return parent[x];
    }

    public bool Union(int a, int b)
    {
        int ra = Find(a);
        int rb = Find(b);

        if (ra == rb)
            return false;

        if (rank[ra] < rank[rb])
            parent[ra] = rb;
        else if (rank[ra] > rank[rb])
            parent[rb] = ra;
        else
        {
            parent[rb] = ra;
            rank[ra]++;
        }

        SetCount--;
        return true;
    }
}

class Cluster
{
    public HashSet<int> regions = new HashSet<int>();
    public Vector2 centroid;
    public int size;

    public Cluster(int region, Vector2 pos)
    {
        regions.Add(region);
        centroid = pos;
        size = 1;
    }

    public static Cluster Merge(Cluster a, Cluster b)
    {
        Cluster c = new Cluster(-1, Vector2.zero);

        c.regions = new HashSet<int>(a.regions);
        c.regions.UnionWith(b.regions);

        c.size = a.size + b.size;
        c.centroid =
            (a.centroid * a.size + b.centroid * b.size) / c.size;

        return c;
    }
}


public class MapView : MonoBehaviour
{
    public GameObject cellPrefab;
    public int voronoiPointCount = 25;

    private MapModel model;
    private GameObject[,] visuals;

    private int[] regionToBiome;
    private const int OceanBiome = -1;
    private const int BiomeCount = 10;

    void Start()
    {
        model = new MapModel();
        model.GenerateVoronoi(voronoiPointCount);

        visuals = new GameObject[MapModel.Width, MapModel.Height];

        DrawGrid();
        ClassifyRegions();
        ApplyColors();
    }

    void DrawGrid()
    {
        for (int x = 0; x < MapModel.Width; x++)
        for (int y = 0; y < MapModel.Height; y++)
        {
            GameObject cell = Instantiate(
                cellPrefab,
                new Vector3(x-5f, y-5f, 0),
                Quaternion.identity,
                transform
            );

            visuals[x, y] = cell;
        }
    }

    float WardDistance(Cluster a, Cluster b)
    {
        float sqrDist = (a.centroid - b.centroid).sqrMagnitude;
        return (a.size * b.size) / (float)(a.size + b.size) * sqrDist;
    }

    void ApplyColors()
    {
        for (int x = 0; x < MapModel.Width; x++)
        for (int y = 0; y < MapModel.Height; y++)
        {
            int region = model.GetCell(x, y);
            int biome = regionToBiome[region];

            SpriteRenderer sr = visuals[x, y].GetComponent<SpriteRenderer>();
            sr.color = BiomeColor(biome);
        }
    }

    Color RegionColor(int region, int regionCount)
    {
        if (region < 0)
            return Color.black;

        float t = (float)region / Mathf.Max(1, regionCount);
        return Color.HSVToRGB(t, 0.6f, 0.9f);
    }

    void AssignBiomesSpatially(System.Collections.Generic.List<int> landRegions)
    {
        var sites = model.GetVoronoi().GetSites();

        // 1. Pick biome seeds from land regions
        var biomeSeeds = new System.Collections.Generic.List<int>();

        while (biomeSeeds.Count < BiomeCount && biomeSeeds.Count < landRegions.Count)
        {
            int candidate = landRegions[Random.Range(0, landRegions.Count)];
            if (!biomeSeeds.Contains(candidate))
                biomeSeeds.Add(candidate);
        }

        // 2. Assign each land region to nearest biome seed
        foreach (int region in landRegions)
        {
            Vector2 p = sites[region];

            float minDist = float.MaxValue;
            int bestBiome = -1;

            for (int b = 0; b < biomeSeeds.Count; b++)
            {
                Vector2 seedPos = sites[biomeSeeds[b]];
                float d = Vector2.Distance(p, seedPos);

                if (d < minDist)
                {
                    minDist = d;
                    bestBiome = b;
                }
            }

            regionToBiome[region] = bestBiome;
        }
    }

void AssignBiomesByClustering(List<int> landRegions)
{
    var sites = model.GetVoronoi().GetSites();

    // Initialize one cluster per region
    List<Cluster> clusters = new List<Cluster>();
    foreach (int region in landRegions)
        clusters.Add(new Cluster(region, sites[region]));

    // Agglomerative clustering
    while (clusters.Count > BiomeCount)
    {
        float bestDist = float.MaxValue;
        int bestA = -1;
        int bestB = -1;

        // Find closest cluster pair (Ward)
        for (int i = 0; i < clusters.Count; i++)
        {
            for (int j = i + 1; j < clusters.Count; j++)
            {
                float d = WardDistance(clusters[i], clusters[j]);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestA = i;
                    bestB = j;
                }
            }
        }

        // Merge
        Cluster merged = Cluster.Merge(clusters[bestA], clusters[bestB]);

        // Remove higher index first
        if (bestA > bestB)
        {
            clusters.RemoveAt(bestA);
            clusters.RemoveAt(bestB);
        }
        else
        {
            clusters.RemoveAt(bestB);
            clusters.RemoveAt(bestA);
        }

        clusters.Add(merged);
    }

    // Assign biome IDs
    for (int biome = 0; biome < clusters.Count; biome++)
    {
        foreach (int region in clusters[biome].regions)
            regionToBiome[region] = biome;
    }
}


    void ClassifyRegions()
    {
        int regionCount = model.GetVoronoi().GetSites().Count;
        regionToBiome = new int[regionCount];

        // Initialize as unassigned
        for (int i = 0; i < regionCount; i++)
            regionToBiome[i] = int.MinValue;

        // 1. Find edge regions → Ocean
        for (int x = 0; x < MapModel.Width; x++)
        {
            MarkOcean(model.GetCell(x, 0));
            MarkOcean(model.GetCell(x, MapModel.Height - 1));
        }

        for (int y = 0; y < MapModel.Height; y++)
        {
            MarkOcean(model.GetCell(0, y));
            MarkOcean(model.GetCell(MapModel.Width - 1, y));
        }

        // 2. Collect land regions
        var landRegions = new System.Collections.Generic.List<int>();
        for (int i = 0; i < regionCount; i++)
        {
            if (regionToBiome[i] == int.MinValue)
                landRegions.Add(i);
        }

        // 3. Assign land regions using hierarchical clustering
        AssignBiomesByClustering(landRegions);
    }

    void MarkOcean(int region)
    {
        if (region >= 0)
            regionToBiome[region] = OceanBiome;
    }

Color BiomeColor(int biome)
{
    // Ocean
    if (biome == OceanBiome)
        return new Color(0.1f, 0.3f, 0.8f);

    switch (biome)
    {
        case 0: // 1. Ice
            return Color.cyan;

        case 1: // 2. Hell
            return new Color(0.5f, 0.0f, 0.0f); // dark red

        case 2: // 3. Light pink
            return new Color(1.0f, 0.7f, 0.8f);

        case 3: // 4. Dark purple
            return new Color(0.4f, 0.0f, 0.5f);

        case 4: // 5. Desert (orange-yellow)
            return new Color(1.0f, 0.8f, 0.4f);

        case 5: // 6. Grass (light green)
            return new Color(0.5f, 0.9f, 0.5f);

        case 6: // 7. Dark forest
            return new Color(0.0f, 0.4f, 0.0f);

        case 7: // 8. Repeat desert
            return new Color(1.0f, 0.8f, 0.4f);

        case 8: // 9. Repeat grass
            return new Color(0.5f, 0.9f, 0.5f);

        case 9: // 10. Repeat dark forest
            return new Color(0.0f, 0.4f, 0.0f);

        default:
            return Color.magenta; // debug fallback
    }
}
}

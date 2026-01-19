using UnityEngine;
using System.Collections.Generic;

public class PoissonObjectGenerator : IObjectGenerator
{
    private int width;
    private int height;
    private float minDistance;
    private int maxAttempts;
    private int objectID;

    private float cellSize;
    private int gridWidth;
    private int gridHeight;

    public PoissonObjectGenerator(
        int width,
        int height,
        float minDistance,
        int maxAttempts,
        int objectID)
    {
        this.width = width;
        this.height = height;
        this.minDistance = minDistance;
        this.maxAttempts = maxAttempts;
        this.objectID = objectID;

        cellSize = minDistance / Mathf.Sqrt(2);
        gridWidth = Mathf.CeilToInt(width / cellSize);
        gridHeight = Mathf.CeilToInt(height / cellSize);
    }

    public List<ObjectData> Generate(BiomeData biome, List<MountainData> mountains)
    {
        List<Vector2> points = GeneratePoints();
        List<ObjectData> result = new List<ObjectData>();

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

    // ---------------- Poisson Sampling ----------------

    private List<Vector2> GeneratePoints()
    {
        Vector2?[,] grid = new Vector2?[gridWidth, gridHeight];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> active = new List<Vector2>();

        // 1. Initial point
        Vector2 start = new Vector2(
            Random.Range(0f, width),
            Random.Range(0f, height));

        AddPoint(start);

        // 2. Main loop
        while (active.Count > 0)
        {
            int index = Random.Range(0, active.Count);
            Vector2 center = active[index];
            bool found = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 candidate = GenerateAround(center);

                if (IsValid(candidate))
                {
                    AddPoint(candidate);
                    found = true;
                    break;
                }
            }

            if (!found)
                active.RemoveAt(index);
        }

        return points;

        // ---------- Local helpers ----------

        void AddPoint(Vector2 p)
        {
            points.Add(p);
            active.Add(p);

            int gx = (int)(p.x / cellSize);
            int gy = (int)(p.y / cellSize);
            grid[gx, gy] = p;
        }

        bool IsValid(Vector2 p)
        {
            if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height)
                return false;

            int gx = (int)(p.x / cellSize);
            int gy = (int)(p.y / cellSize);

            for (int x = Mathf.Max(0, gx - 2); x <= Mathf.Min(gridWidth - 1, gx + 2); x++)
            for (int y = Mathf.Max(0, gy - 2); y <= Mathf.Min(gridHeight - 1, gy + 2); y++)
            {
                if (grid[x, y].HasValue &&
                    Vector2.Distance(grid[x, y].Value, p) < minDistance)
                    return false;
            }

            return true;
        }

        Vector2 GenerateAround(Vector2 p)
        {
            float angle = Random.value * Mathf.PI * 2;
            float radius = Random.Range(minDistance, 2 * minDistance);
            return p + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }
    }
}

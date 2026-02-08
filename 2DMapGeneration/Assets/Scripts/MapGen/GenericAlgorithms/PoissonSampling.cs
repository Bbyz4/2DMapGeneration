using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates Poisson-disk distributed points in 2D space
/// using Bridson's algorithm.
/// </summary>
public class PoissonSampling
{
    public float left  { get; private set; }
    public float right { get; private set; }
    public float top    { get; private set; }
    public float bottom  { get; private set; }

    private float minDistance;
    private int maxPoints;
    private int rejectionSamples;
    private int initialPoints;

    private float cellSize;
    private Vector2[,] grid;

    private List<Vector2> points;
    private List<Vector2> activePoints;

    /// <summary>
    /// Creates a Poisson disk sampler within bounds.
    /// </summary>
    public PoissonSampling(
        float left,
        float right,
        float bottom,
        float top,
        float minDistance,
        int maxPoints,
        int rejectionSamples,
        int initialPoints)
    {
        this.left = left;
        this.right = right;
        this.bottom = bottom;
        this.top = top;

        this.minDistance = minDistance;
        this.maxPoints = maxPoints;
        this.rejectionSamples = rejectionSamples;
        this.initialPoints = initialPoints;

        cellSize = minDistance / Mathf.Sqrt(2);

        int gridWidth = Mathf.CeilToInt((right - left) / cellSize);
        int gridHeight = Mathf.CeilToInt((top - bottom) / cellSize);

        grid = new Vector2[gridWidth, gridHeight];
        points = new List<Vector2>();
        activePoints = new List<Vector2>();
    }

    /// <summary>
    /// Generates and returns Poisson-distributed points.
    /// </summary>
    public List<Vector2> Generate()
    {
        points.Clear();
        activePoints.Clear();

        // Initial random point
        int seeds = Mathf.Min(initialPoints, maxPoints);
        int attempts = 0;

        while (points.Count < seeds && attempts < seeds * 10)
        {
            Vector2 candidate = new Vector2(
                Random.Range(left, right),
                Random.Range(bottom, top)
            );

            if (IsValid(candidate))
            {
                AddPoint(candidate);
            }

            attempts++;
        }

        while (activePoints.Count > 0 && points.Count < maxPoints)
        {
            int index = Random.Range(0, activePoints.Count);
            Vector2 center = activePoints[index];
            bool found = false;

            for (int i = 0; i < rejectionSamples; i++)
            {
                Vector2 candidate = GenerateCandidate(center);

                if (IsValid(candidate))
                {
                    AddPoint(candidate);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                activePoints.RemoveAt(index);
            }
        }

        return new List<Vector2>(points);
    }

    public List<Vector2> GetPreviouslyGeneratedPoints()
    {
        return new List<Vector2>(points);
    }

    /// <summary>
    /// Adds a point to internal structures.
    /// </summary>
    private void AddPoint(Vector2 point)
    {
        points.Add(point);
        activePoints.Add(point);

        Vector2Int cell = WorldToGrid(point);
        grid[cell.x, cell.y] = point;
    }

    /// <summary>
    /// Generates a random candidate around a point.
    /// </summary>
    private Vector2 GenerateCandidate(Vector2 center)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minDistance, 2f * minDistance);

        return center + new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ) * radius;
    }

    /// <summary>
    /// Checks if a point is valid against bounds and neighbors.
    /// </summary>
    private bool IsValid(Vector2 point)
    {
        if (point.x < left || point.x >= right ||
            point.y < bottom || point.y >= top)
            return false;

        Vector2Int cell = WorldToGrid(point);

        int searchRadius = 2;

        for (int x = Mathf.Max(0, cell.x - searchRadius);
             x <= Mathf.Min(grid.GetLength(0) - 1, cell.x + searchRadius);
             x++)
        {
            for (int y = Mathf.Max(0, cell.y - searchRadius);
                 y <= Mathf.Min(grid.GetLength(1) - 1, cell.y + searchRadius);
                 y++)
            {
                Vector2 neighbor = grid[x, y];
                if (neighbor != Vector2.zero)
                {
                    if (Vector2.Distance(point, neighbor) < minDistance)
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Converts world position to grid index.
    /// </summary>
    private Vector2Int WorldToGrid(Vector2 point)
    {
        int x = Mathf.FloorToInt((point.x - left) / cellSize);
        int y = Mathf.FloorToInt((point.y - bottom) / cellSize);
        return new Vector2Int(x, y);
    }
}

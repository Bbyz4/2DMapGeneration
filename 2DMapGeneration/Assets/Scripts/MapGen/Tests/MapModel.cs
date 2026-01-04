using UnityEngine;

public class MapModel
{
    public const int Width = 300;
    public const int Height = 300;

    private int[,] grid;
    private VoronoiDiagram voronoi;

    public MapModel()
    {
        grid = new int[Width, Height];

        // Voronoi bounds match grid coordinates
        voronoi = new VoronoiDiagram(
            left: 0,
            right: Width,
            up: Height,
            down: 0
        );
    }

    /// <summary>
    /// Generates Voronoi-based regions for the entire grid.
    /// </summary>
    public void GenerateVoronoi(int pointCount)
    {
        voronoi.Generate(
            pointCount,
            Vector2.Distance // Euclidean distance
        );

        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
        {
            // Sample at cell center
            float fx = x + 0.5f;
            float fy = y + 0.5f;

            int region = voronoi.GetRegionIndex(fx, fy);
            grid[x, y] = region;
        }
    }

    /// <summary>
    /// Returns the region index at grid position.
    /// </summary>
    public int GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public VoronoiDiagram GetVoronoi()
    {
        return voronoi;
    }
}

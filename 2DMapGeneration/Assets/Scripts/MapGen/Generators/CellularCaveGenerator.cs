using UnityEngine;
using System.Collections.Generic;

public class CellularCaveGenerator : IMountainGenerator
{
    private CellularAutomata automata;

    private int width;
    private int height;

private List<Vector2Int> FloodFill(
        int startX,
        int startY,
        int level,
        int[,] map,
        bool[,] visited)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        while(queue.Count > 0)
        {
            Vector2Int p = queue.Dequeue();
            cells.Add(p);

            foreach(Vector2Int n in Neighbors(p))
            {
                if(n.x < 0 || n.x >= width ||
                   n.y < 0 || n.y >= height)
                    continue;

                if(visited[n.x, n.y])
                    continue;

                if(map[n.x, n.y] != level)
                    continue;

                visited[n.x, n.y] = true;
                queue.Enqueue(n);
            }
        }

        return cells;
    }

    private struct Edge
    {
        public Vector2 a;
        public Vector2 b;

        public Edge(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }
    }

    private IEnumerable<Vector2Int> Neighbors(Vector2Int p)
    {
        yield return new Vector2Int(p.x + 1, p.y);
        yield return new Vector2Int(p.x - 1, p.y);
        yield return new Vector2Int(p.x, p.y + 1);
        yield return new Vector2Int(p.x, p.y - 1);
    }

    private List<Vector2> BuildOutline(List<Vector2Int> region)
    {
        HashSet<Vector2Int> regionSet = new HashSet<Vector2Int>(region);
        List<Edge> edges = new List<Edge>();

        foreach (var cell in region)
        {
            int x = cell.x;
            int y = cell.y;

            // Left
            if (!regionSet.Contains(new Vector2Int(x - 1, y)))
                edges.Add(new Edge(
                    new Vector2(x, y),
                    new Vector2(x, y + 1)));

            // Right
            if (!regionSet.Contains(new Vector2Int(x + 1, y)))
                edges.Add(new Edge(
                    new Vector2(x + 1, y + 1),
                    new Vector2(x + 1, y)));

            // Bottom
            if (!regionSet.Contains(new Vector2Int(x, y - 1)))
                edges.Add(new Edge(
                    new Vector2(x + 1, y),
                    new Vector2(x, y)));

            // Top
            if (!regionSet.Contains(new Vector2Int(x, y + 1)))
                edges.Add(new Edge(
                    new Vector2(x, y + 1),
                    new Vector2(x + 1, y + 1)));
        }

        return OrderEdges(edges);
    }

    private List<Vector2> OrderEdges(List<Edge> edges)
    {
        List<Vector2> outline = new List<Vector2>();

        Edge current = edges[0];
        outline.Add(current.a);
        outline.Add(current.b);
        edges.RemoveAt(0);

        while (edges.Count > 0)
        {
            Vector2 last = outline[outline.Count - 1];

            int index = edges.FindIndex(e => e.a == last || e.b == last);
            if (index == -1)
                break;

            Edge next = edges[index];
            edges.RemoveAt(index);

            outline.Add(next.a == last ? next.b : next.a);
        }

        outline.RemoveAt(outline.Count - 1);

        return outline;
    }

    public CellularCaveGenerator(
        int width,
        int height
    )
    {
        this.width = width;
        this.height = height;

        automata = new CellularAutomata(0, width-1, height-1, 0, 2);

        //Setting the edges as 1 (wall)
        for(int i=0; i<height; i++)
        {
            automata.SetCell(0, i, 1);
            automata.SetCell(width-1, i, 1);
        }

        for(int i=0; i<width; i++)
        {
            automata.SetCell(i, 0, 1);
            automata.SetCell(i, height-1, 1);
        }
    }

    public List<MountainData> Generate(BiomeData biome)
    {
        List<MountainData> result = new List<MountainData>();

        automata.Randomize();

        automata.SetTransitionRule((grid, x, y) =>
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            int sum = 0;
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && (nx!=x || ny!=y))
                    {
                        sum += grid[nx, ny];
                        count++;
                    }
                }
            }

            int newValue;

            //Classic cavegen

            if(grid[x,y] == 1)
            {
                //Wall stays wall if >=4 neighbours are wall
                newValue = (sum >= 4) ? 1 : 0;
            }
            else
            {
                //Floor becomes wall if >=5 neighbours are wall
                newValue = (sum >= 5) ? 1 : 0;
            }

            return newValue;
        });

        automata.ApplyTransition(8);

        int[,] elevationMap = new int[width, height];
        bool[,] visited = new bool[width, height];

        // 1. Sample noise → elevation map
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int h = automata.GetCell(x, y);
                elevationMap[x, y] = h; // no need to classify, 1 is already mountain and 0 is already empty
            }
        }

        // 2. Extract connected regions
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(visited[x, y])
                    continue;

                int level = elevationMap[x, y];

                // Ignore flat land
                if(level == 0)
                {
                    visited[x, y] = true;
                    continue;
                }

                List<Vector2Int> region = FloodFill(
                    x, y, level, elevationMap, visited);

                MountainData mountain = new MountainData
                {
                    elevationLevel = level,
                    outline = BuildOutline(region)
                };

                result.Add(mountain);
            }
        }

        return result;
    }
}
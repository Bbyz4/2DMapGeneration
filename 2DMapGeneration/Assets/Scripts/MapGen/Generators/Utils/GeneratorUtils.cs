using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneratorUtils
{
    /// ------------------------------------------------------------------------------------------------
    /// Private methods
    /// ------------------------------------------------------------------------------------------------

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

    private static IEnumerable<Vector2Int> Neighbors(Vector2Int p)
    {
        yield return new Vector2Int(p.x + 1, p.y);
        yield return new Vector2Int(p.x - 1, p.y);
        yield return new Vector2Int(p.x, p.y + 1);
        yield return new Vector2Int(p.x, p.y - 1);
    }

    private static List<Vector2> OrderEdges(List<Edge> edges)
    {
        if(edges.Count == 0)
        {
            return new List<Vector2>();
        }

        Dictionary<Vector2, List<Vector2>> VertexGraph = new Dictionary<Vector2, List<Vector2>>();

        foreach(Edge e in edges)
        {
            if(!VertexGraph.ContainsKey(e.a))
            {
                VertexGraph[e.a] = new List<Vector2>();
            }

            if(!VertexGraph.ContainsKey(e.b))
            {
                VertexGraph[e.b] = new List<Vector2>();
            }

            VertexGraph[e.a].Add(e.b);
            VertexGraph[e.b].Add(e.a);
        }

        //Left-lowest vertex
        Vector2 startVertex = edges[0].a;

        foreach(Edge e in edges)
        {
            if(e.a.x < startVertex.x || e.a.x == startVertex.x && e.a.y < startVertex.y)
            {
                startVertex = e.a;
            }

            if(e.b.x < startVertex.x || e.b.x == startVertex.x && e.b.y < startVertex.y)
            {
                startVertex = e.b;
            }
        }

        //Fake prevVertex
        Vector2 prev = new Vector2(startVertex.x - 1, startVertex.y);
        Vector2 current = startVertex;

        List<Vector2> outline = new List<Vector2>();

        int stepCounter = 0;
        int MAX_STEPS = 10000;

        do
        {
           outline.Add(current); 

           List<Vector2> neighbours = VertexGraph[current];

           Vector2 incomingDirection = (current - prev).normalized;

           Vector2 bestNeigh = neighbours[0];
           float bestAngle = float.MinValue;

           foreach(Vector2 neigh in neighbours)
           {
                if(neigh == prev)
                {
                    continue;
                }

                Vector2 direction = (neigh - current).normalized;

                float angle = Vector2.SignedAngle(incomingDirection, direction);

                angle -= (angle > 91f) ? 360f : 0f;

                if(angle > bestAngle)
                {
                    bestAngle = angle;
                    bestNeigh = neigh;
                }
           }

           prev = current;
           current = bestNeigh;

           stepCounter++;
           if(stepCounter > MAX_STEPS)
            {
                Debug.LogError("OrderEdges fail");
                break;
            }
        }
        while(current != startVertex);

        return outline;
    }

    private static List<Vector2> BuildOutline(List<Vector2Int> region, int xAddedToAllOutlines = 0, int yAddedToAllOutlines = 0)
    {
        HashSet<Vector2Int> regionSet = new HashSet<Vector2Int>(region);
        List<Edge> edges = new List<Edge>();

        foreach (var cell in region)
        {
            int x = cell.x;
            int y = cell.y;

            int fx = x + xAddedToAllOutlines;
            int fy = y + yAddedToAllOutlines;

            // Left
            if (!regionSet.Contains(new Vector2Int(x - 1, y)))
                edges.Add(new Edge(
                    new Vector2(fx, fy),
                    new Vector2(fx, fy + 1)));

            // Right
            if (!regionSet.Contains(new Vector2Int(x + 1, y)))
                edges.Add(new Edge(
                    new Vector2(fx + 1, fy + 1),
                    new Vector2(fx + 1, fy)));

            // Bottom
            if (!regionSet.Contains(new Vector2Int(x, y - 1)))
                edges.Add(new Edge(
                    new Vector2(fx + 1, fy),
                    new Vector2(fx, fy)));

            // Top
            if (!regionSet.Contains(new Vector2Int(x, y + 1)))
                edges.Add(new Edge(
                    new Vector2(fx, fy + 1),
                    new Vector2(fx + 1, fy + 1)));
        }

        return OrderEdges(edges);
    }

    private static List<Vector2Int> FloodFill(int startX, int startY, int level, int[,] map, bool[,] visited, int width, int height)
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

    public static List<MountainData> BuildMountainOutlines(int[,] elevationMap, int width, int height, int xAddedToAllOutlines = 0, int yAddedToAllOutlines = 0)
    {
        List<MountainData> result = new List<MountainData>();

        bool[,] visited = new bool[width, height];

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                if(visited[x,y])
                {
                    continue;
                }

                int level = elevationMap[x, y];

                // Ignore flat land
                if(level == 0)
                {
                    visited[x, y] = true;
                    continue;
                }

                List<Vector2Int> region = FloodFill(x,y,level,elevationMap,visited,width,height);

                MountainData mountain = new MountainData
                {
                    elevationLevel = level,
                    outline = BuildOutline(region, xAddedToAllOutlines, yAddedToAllOutlines)
                };

                result.Add(mountain);
            }
        }

        return result;
    }

    public static List<BiomeData> BuildBiomeOutlines(int[,] biomeTable, int width, int height)
    {
        List<BiomeData> result = new List<BiomeData>();

        bool[,] visited = new bool[width, height];

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                if(visited[x,y])
                {
                    continue;
                }

                int level = biomeTable[x, y];

                // Convention: Ignore tiles that are market with a negative number
                if(level < 0)
                {
                    visited[x, y] = true;
                    continue;
                }

                List<Vector2Int> region = FloodFill(x,y,level,biomeTable,visited,width,height);

                BiomeData biome = new BiomeData
                {
                    biomeID = -1,
                    outline = BuildOutline(region)
                };

                result.Add(biome);
            }
        }

        return result;
    }

}
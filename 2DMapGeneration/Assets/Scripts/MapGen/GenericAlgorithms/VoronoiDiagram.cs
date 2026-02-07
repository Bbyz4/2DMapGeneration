using UnityEngine;
using System.Collections.Generic;
using System;

public class VoronoiDiagram
{
    public int left  { get; private set; }
    public int right { get; private set; }
    public int up    { get; private set; }
    public int down  { get; private set; }

    private List<Vector2> sites;
    private HashSet<Vector2> manualSites;

    public VoronoiDiagram(int left, int right, int up, int down)
    {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;

        sites = new List<Vector2>();
        manualSites = new HashSet<Vector2>();
    }

    /// <summary>
    /// Manually add a Voronoi site.
    /// </summary>
    public void AddPoint(Vector2 point)
    {
        if(!IsWithinBounds(point))
        {
            return;
        }

        if(!manualSites.Contains(point))
        {
            manualSites.Add(point);
            sites.Add(point);
        }
    }

    /// <summary>
    /// Remove a manually added Voronoi site.
    /// </summary>
    public void RemovePoint(Vector2 point)
    {
        if(manualSites.Remove(point))
        {
            sites.Remove(point);
        }
    }

    /// <summary>
    /// Generates Voronoi sites.
    /// Includes all manual points and adds random ones.
    /// </summary>
    public void Generate(int randomPointCount, Func<Vector2, Vector2, float> distanceFunction)
    {
        this.distanceFunction = distanceFunction;

        // Remove old random points
        sites.RemoveAll(p => !manualSites.Contains(p));

        for(int i = 0; i < randomPointCount; i++)
        {
            Vector2 p = new Vector2(
                UnityEngine.Random.Range(left, right),
                UnityEngine.Random.Range(down, up)
            );

            sites.Add(p);
        }
    }

    private Func<Vector2, Vector2, float> distanceFunction;

    /// <summary>
    /// Returns the index of the Voronoi region for a continuous point.
    /// </summary>
    public int GetRegionIndex(float x, float y)
    {
        Vector2 p = new Vector2(x, y);

        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for(int i = 0; i < sites.Count; i++)
        {
            float d = distanceFunction(p, sites[i]);

            if(d < minDistance)
            {
                minDistance = d;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    /// <summary>
    /// Returns the site (generator point) for the Voronoi region.
    /// </summary>
    public Vector2 GetRegionSite(float x, float y)
    {
        int index = GetRegionIndex(x, y);
        return index >= 0 ? sites[index] : Vector2.zero;
    }

    public IReadOnlyList<Vector2> GetSites()
    {
        return sites;
    }

    private bool IsWithinBounds(Vector2 p)
    {
        return p.x >= left && p.x <= right && p.y >= down && p.y <= up;
    }
}

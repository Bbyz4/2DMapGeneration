using UnityEngine;
using System.Collections.Generic;

public class PerlinNoise
{
    private int left;
    private int right;
    private int up;
    private int down;

    private Dictionary<Vector2Int, Vector2> gradients;
    private HashSet<Vector2Int> manualOverrides;

    /// <summary>
    /// Creates a Perlin-style gradient grid bounded by integer coordinates.
    /// </summary>
    public PerlinNoise(int left, int right, int up, int down)
    {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;

        gradients = new Dictionary<Vector2Int, Vector2>();
        manualOverrides = new HashSet<Vector2Int>();

        Randomize();
    }

    /// <summary>
    /// Assigns random unit gradient vectors to all grid points
    /// </summary>
    public void Randomize()
    {
        for(int x = left; x <= right; x++)
        {
            for(int y = down; y <= up; y++)
            {
                Vector2Int key = new Vector2Int(x, y);

                if(manualOverrides.Contains(key))
                {
                    continue;
                }

                gradients[key] = RandomUnitVector();
            }
        }
    }

    /// <summary>
    /// Manually sets a gradient (arrow) at a specific grid point.
    /// </summary>
    public void SetManualGradient(int x, int y, Vector2 gradient)
    {
        Vector2Int key = new Vector2Int(x, y);

        if(!IsWithinBounds(x, y))
            return;

        gradients[key] = gradient.normalized;
        manualOverrides.Add(key);
    }

    /// <summary>
    /// Removes a manual override and restores random behavior at this point.
    /// </summary>
    public void ClearManualGradient(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);

        if(!IsWithinBounds(x, y))
            return;

        manualOverrides.Remove(key);
        gradients[key] = RandomUnitVector();
    }

    /// <summary>
    /// Continuous Perlin noise sampling.
    /// Returns a value in [0, 1].
    /// </summary>
    public float Sample(float x, float y)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        x0 = Mathf.Clamp(x0, left, right - 1);
        y0 = Mathf.Clamp(y0, down, up - 1);
        x1 = x0 + 1;
        y1 = y0 + 1;

        Vector2 g00 = GetGradient(x0, y0);
        Vector2 g10 = GetGradient(x1, y0);
        Vector2 g01 = GetGradient(x0, y1);
        Vector2 g11 = GetGradient(x1, y1);

        Vector2 d00 = new Vector2(x - x0, y - y0);
        Vector2 d10 = new Vector2(x - x1, y - y0);
        Vector2 d01 = new Vector2(x - x0, y - y1);
        Vector2 d11 = new Vector2(x - x1, y - y1);

        float v00 = Vector2.Dot(g00, d00);
        float v10 = Vector2.Dot(g10, d10);
        float v01 = Vector2.Dot(g01, d01);
        float v11 = Vector2.Dot(g11, d11);

        float u = Fade(d00.x);
        float v = Fade(d00.y);

        float ix0 = Mathf.Lerp(v00, v10, u);
        float ix1 = Mathf.Lerp(v01, v11, u);
        float value = Mathf.Lerp(ix0, ix1, v);

        return Mathf.Clamp01((value + 1f) * 0.5f);
    }

    /// <summary>
    /// Returns the gradient vector at a grid point.
    /// </summary>
    public Vector2 GetGradient(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);

        if(gradients.TryGetValue(key, out Vector2 gradient))
        {
            return gradient;
        }

        return Vector2.zero;
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= left && x <= right && y >= down && y <= up;
    }

    private Vector2 RandomUnitVector()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    /// <summary>
    /// Ken Perlin's original fade function (C2 continuous)
    /// </summary>
    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}

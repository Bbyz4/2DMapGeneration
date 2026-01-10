using UnityEngine;
using System.Collections.Generic;
using System;

public class CellularAutomata
{
    private int left;
    private int right;
    private int up;
    private int down;

    private int width, height;

    private int distinctStates;

    private int[,] grid;
    private int[,] nextGrid;

    private HashSet<Vector2Int> manualOverrides;

    private Func<int[,], int, int, int> transitionRule;

    /// <summary>
    /// Creates a Cellular Automata grid bounded by given coords with n distinct states
    /// </summary>
    public CellularAutomata(int left, int right, int up, int down, int distinctStates)
    {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
        this.distinctStates = distinctStates;

        width = right - left + 1;
        height = up - down + 1;

        grid = new int[width, height];
        nextGrid = new int[width, height];
        manualOverrides = new HashSet<Vector2Int>();


    }

    /// <summary>
    /// Randomly assigns states to all cells, except for the manually overwritten ones
    /// </summary>
    public void Randomize()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2Int key = new Vector2Int(x + left, y + down);
                if(!manualOverrides.Contains(key))
                {
                    grid[x, y] = UnityEngine.Random.Range(0, distinctStates);
                }
            }
        }
    }

    /// <summary>
    /// Sets a specific cell to a given state manually
    /// </summary>
    public void SetCell(int x, int y, int state)
    {
        if(!IsWithinBounds(x, y) || state < 0 || state >= distinctStates)
        {
            return;
        }

        Vector2Int key = new Vector2Int(x, y);
        grid[x - left, y - down] = state;
        manualOverrides.Add(key);
    }

    /// <summary>
    /// Clears manual override for a cell and optionally randomizes its value
    /// </summary>
    public void ClearManualCell(int x, int y, bool randomize = true)
    {
        if(!IsWithinBounds(x, y))
        {
            return;
        }

        Vector2Int key = new Vector2Int(x, y);
        manualOverrides.Remove(key);

        if(randomize)
        {
            grid[x - left, y - down] = UnityEngine.Random.Range(0, distinctStates);
        }
    }

    /// <summary>
    /// Returns the state at a specific coordinate
    /// </summary>
    public int GetCell(int x, int y)
    {
        if(!IsWithinBounds(x, y))
        {
            return 0;
        }
        return grid[x - left, y - down];
    }

    /// <summary>
    /// Sets the transition rule function
    /// It should take (grid, x, y) and return the new state
    /// </summary>
    public void SetTransitionRule(Func<int[,], int, int, int> rule)
    {
        transitionRule = rule;
    }

    public void ApplyTransition(int steps = 1)
    {
        if(transitionRule == null)
        {
            return;
        }

        for(int s=0; s < steps; s++)
        {
            for(int x=0; x < width; x++)
            {
                for(int y=0; y < height; y++)
                {
                    Vector2Int key = new Vector2Int(x + left, y + down);
                    nextGrid[x,y] = Mathf.Clamp(transitionRule(grid, x, y), 0, distinctStates-1);
                }
            }

            var temp = grid;
            grid = nextGrid;
            nextGrid = temp;
        }
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= left && x <= right && y >= down && y <= up;
    }
}
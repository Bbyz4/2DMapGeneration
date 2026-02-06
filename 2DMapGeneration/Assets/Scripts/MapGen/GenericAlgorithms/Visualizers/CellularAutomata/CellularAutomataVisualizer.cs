using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataVisualizer : MonoBehaviour
{

    private CellularAutomata AUTOMATA;
    private Dictionary<int, Color> COLOR_MAP;
    private float VIS_SCALE;
    private Vector2 VIS_MOVEMENT;

    [SerializeField] private GameObject cellTilePrefab;


    private GameObject[,] generatedSquares;

    private int left;
    private int right;
    private int up;
    private int down;

    public void Initialize(CellularAutomata automata, Dictionary<int, Color> colorMap, float visScale = 1f, float visMovementX = 0f, float visMovementY = 0f)
    {
        this.AUTOMATA = automata;
        this.COLOR_MAP = colorMap;
        this.VIS_SCALE = visScale;
        this.VIS_MOVEMENT = new Vector2(visMovementX, visMovementY);

        this.left = automata.left;
        this.right = automata.right;
        this.up = automata.up;
        this.down = automata.down;

        this.generatedSquares = new GameObject[right - left + 1, up - down + 1];
        GenerateStartingSquares();
    }

    private void GenerateStartingSquares()
    {
        for(int i = left; i <= right; i++)
        {
            for(int j = down; j <= up; j++)
            {
                generatedSquares[i-left, j-down] = Instantiate(cellTilePrefab, new Vector3(i, j, 0f), Quaternion.identity, transform); 
            }
        }
    }

    public void VisStep()
    {
        for(int i=left; i<=right; i++)
        {
            for(int j=down; j<=up; j++)
            {
                generatedSquares[i-left, j-down].GetComponent<SpriteRenderer>().color = COLOR_MAP[AUTOMATA.GetCell(i, j)];
            }
        }
    }

}
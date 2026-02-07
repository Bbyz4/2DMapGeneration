using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagramVisualizer : MonoBehaviour
{
    private VoronoiDiagram DIAGRAM;
    
    private Dictionary<int, Color> COLOR_MAP;
    private float VIS_SCALE;
    private Vector2 VIS_MOVEMENT;

    [SerializeField] private GameObject fieldPrefab;
    [SerializeField] private GameObject sitePrefab;

    private GameObject[,] generatedFields;
    private GameObject[,] generatedSites;

    private int left;
    private int right;
    private int up;
    private int down;

    public void Initialize(VoronoiDiagram diagram, Dictionary<int, Color> colorMap, float visScale = 1f, float visMovementX = 0f, float visMovementY = 0f)
    {
        this.DIAGRAM = diagram;
        this.COLOR_MAP = colorMap;
        this.VIS_SCALE = visScale;
        this.VIS_MOVEMENT = new Vector2(visMovementX, visMovementY);

        this.left = diagram.left;
        this.right = diagram.right;
        this.up = diagram.up;
        this.down = diagram.down;

        this.generatedFields = new GameObject[right - left + 1, up - down + 1];
        this.generatedSites = new GameObject[right - left + 1, up - down + 1];
        GenerateStartingSquares();
    }

    private void GenerateStartingSquares()
    {
        for(int i = left; i <= right; i++)
        {
            for(int j = down; j <= up; j++)
            {
                generatedFields[i-left, j-down] = Instantiate(fieldPrefab, new Vector3(i, j, 0f), Quaternion.identity, transform); 
            }
        }
    }

    public IEnumerator VisStep()
    {
        for(int i=left; i<=right; i++)
        {
            for(int j=down; j<=up; j++)
            {
                generatedFields[i-left, j-down].GetComponent<SpriteRenderer>().color = COLOR_MAP[DIAGRAM.GetRegionIndex(i,j)];

                if(new Vector2(i,j) == DIAGRAM.GetRegionSite(i,j))
                {
                    generatedSites[i-left, j-down] = Instantiate(sitePrefab, new Vector3(i, j, 0f), Quaternion.identity, transform); 
                }
            }
        }

        yield return new WaitForSeconds(1f);
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap mountainTilemap;

    public void DrawMountains(int[,] elevationMap, BiomeBehaviour biome, int startX, int startY)
    {
        int width = elevationMap.GetLength(0);
        int height = elevationMap.GetLength(1);

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                if(OutlineUtils.IsPointInOutline(new Vector2(x+startX, y+startY), biome.GetData().outline))
                {
                    int level = elevationMap[x,y];

                    if(level == 0)
                    {
                        continue;
                    }

                    TileBase tile = biome.GetCharacteristics().GetMountainTurf(level);

                    mountainTilemap.SetTile(new Vector3Int(startX+x, startY+y,0), tile);
                }
            }
        }
    }
}

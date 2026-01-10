using System.Collections.Generic;
using UnityEngine;

public class ShapeDrawTest : MonoBehaviour
{
    [SerializeField] private Texture2D shapeTexture;
    [SerializeField] private List<Vector2> shapeOutline;

    [SerializeField] private List<Texture2D> mountainLevelTextures;

    private MapJSONBuilder mJSONb;

    int MAP_SIZE = 100;

    void Awake()
    {
        mJSONb = new MapJSONBuilder(new Vector2(MAP_SIZE, MAP_SIZE));

        if (shapeOutline != null && shapeTexture != null)
        {
            PerlinMountainGenerator pmg = new PerlinMountainGenerator(new PerlinNoise(0, MAP_SIZE, MAP_SIZE, 0), MAP_SIZE, MAP_SIZE, 0.025f, 0.35f, 0.55f, 0.6f, 0.7f);

            List<MountainData> mountains = pmg.Generate(null);

            /* CellularMountainGenerator cmg = new CellularMountainGenerator(MAP_SIZE, MAP_SIZE, 5, 12, 13, 14, 20);

            List<MountainData> mountains = cmg.Generate(null); */

            for(int i=0; i<mountains.Count; i++)
            {
                GameObject newDrawnMount = OutlineUtils.CreateShapeObject($"Mountain_{i}", mountains[i].outline, mountainLevelTextures[mountains[i].elevationLevel + 1], this.transform, Color.black, 0.1f);
                mJSONb.AddMountain(mountains[i]);

                float z = -mountains[i].elevationLevel * 0.1f;
                newDrawnMount.transform.localPosition += Vector3.forward * z;
            }
        }

        mJSONb.SaveToJson("Assets/Scripts/MapGen/Tests/test_map.json");
    }

    // Helper: simple radial polygon (convex, good for testing)
    List<Vector2> GenerateRandomPolygon(Vector2 center, float radius, int pointCount)
    {
        List<Vector2> points = new();

        float angleStep = 360f / pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            float r = radius * Random.Range(0.7f, 1.1f);

            Vector2 p = center + new Vector2(
                Mathf.Cos(angle) * r,
                Mathf.Sin(angle) * r
            );

            points.Add(p);
        }

        return points;
    }
}
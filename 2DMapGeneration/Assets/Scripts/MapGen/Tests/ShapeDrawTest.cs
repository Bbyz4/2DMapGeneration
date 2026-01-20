using System.Collections.Generic;
using UnityEngine;

public class ShapeDrawTest : MonoBehaviour
{
    [SerializeField] private Texture2D shapeTexture;
    [SerializeField] private List<Vector2> shapeOutline;

    [SerializeField] private List<Texture2D> mountainLevelTextures;

    private MapJSONBuilder mJSONb;

    int MAP_SIZE = 200;

    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject rockPrefab;

    void Awake()
    {
        mJSONb = new MapJSONBuilder(new Vector2(MAP_SIZE, MAP_SIZE));

        if (shapeOutline != null && shapeTexture != null)
        {
            IMountainGenerator pmg = new PerlinMountainGenerator(new PerlinNoise(0, MAP_SIZE, MAP_SIZE, 0), MAP_SIZE, MAP_SIZE, 0.025f, 0.35f, 0.55f, 0.6f, 0.7f);

            List<MountainData> mountains = pmg.Generate(null);

            /* CellularCaveGenerator cvg = new CellularCaveGenerator(MAP_SIZE, MAP_SIZE);

            List<MountainData> mountains = cvg.Generate(null); */

            for(int i=0; i<mountains.Count; i++)
            {
                GameObject newDrawnMount = OutlineUtils.CreateShapeObject($"Mountain_{i}", mountains[i].outline, mountainLevelTextures[mountains[i].elevationLevel + 1], this.transform, Color.black, 0.1f);
                mJSONb.AddMountain(mountains[i]);

                float z = -mountains[i].elevationLevel * 0.1f;
                newDrawnMount.transform.localPosition += Vector3.forward * z;
            }

            IObjectGenerator pog = new PoissonObjectGenerator(MAP_SIZE, MAP_SIZE, 1.5f, 30, 1000, 10, 1);

            List<ObjectData> trees = pog.Generate(null, mountains);

            foreach (var tree in trees)
            {
                GameObject t = Instantiate(treePrefab, new Vector3(tree.position.x, tree.position.y, -0.05f), Quaternion.identity, transform);

                mJSONb.AddObject(tree);
            }

            IObjectGenerator pog2 = new PoissonObjectGenerator(MAP_SIZE, MAP_SIZE, 1.5f, 30, 1000, 10, 2);

            List<ObjectData> rocks = pog.Generate(null, mountains);

            foreach (var rock in rocks)
            {
                GameObject t = Instantiate(rockPrefab, new Vector3(rock.position.x, rock.position.y, -0.05f), Quaternion.identity, transform);

                mJSONb.AddObject(rock);
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
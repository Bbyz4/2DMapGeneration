using System.Collections.Generic;
using UnityEngine;

public class ShapeDrawTest : MonoBehaviour
{
    [SerializeField] private Texture2D shapeTexture;
    [SerializeField] private List<Vector2> shapeOutline;

    void Awake()
    {
        if (shapeOutline != null && shapeTexture != null)
        {
            OutlineUtils.CreateShapeObject("TestShape", shapeOutline, shapeTexture, this.transform, Color.black, 0.1f);


            //RunTest();
        }
    }

    void RunTest()
    {
        Vector2 mapSize = new Vector2(100, 100);
        MapJSONBuilder builder = new MapJSONBuilder(mapSize);

        // --- BIOME ---
        BiomeData biome = new BiomeData
        {
            biomeID = 1,
            outline = GenerateRandomPolygon(
                center: new Vector2(50, 50),
                radius: 30,
                pointCount: 8
            )
        };
        builder.AddBiome(biome);

        // --- MOUNTAIN ---
        MountainData mountain = new MountainData
        {
            elevationLevel = 3,
            outline = GenerateRandomPolygon(
                center: new Vector2(50, 50),
                radius: 12,
                pointCount: 6
            )
        };
        builder.AddMountain(mountain);

        // --- OBJECTS ---
        for (int i = 0; i < 5; i++)
        {
            ObjectData obj = new ObjectData
            {
                objectID = Random.Range(1, 4), // tree / rock / etc
                position = new Vector2(
                    Random.Range(20, 80),
                    Random.Range(20, 80)
                )
            };

            builder.AddObject(obj);
        }

        // --- SAVE ---
        string path = Application.dataPath + "/test_map.json";
        builder.SaveToJson(path);
        Debug.Log("Map saved to: " + path);

        // --- LOAD ---
        MapData loaded = MapJSONBuilder.LoadFromJson(path);
        Debug.Log($"Loaded map: Biomes={loaded.biomes.Count}, Mountains={loaded.mountains.Count}, Objects={loaded.objects.Count}");
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
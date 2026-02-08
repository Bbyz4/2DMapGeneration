using System.Collections;
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
/*         mJSONb = new MapJSONBuilder(new Vector2(MAP_SIZE, MAP_SIZE));

        if (shapeOutline != null && shapeTexture != null)
        {
            IMountainGenerator pmg = new PerlinMountainGenerator(new PerlinNoise(0, MAP_SIZE, MAP_SIZE, 0), MAP_SIZE, MAP_SIZE, 0.025f, 0.35f, 0.55f, 0.6f, 0.7f);

            List<MountainData> mountains = pmg.Generate(null);

            for(int i=0; i<mountains.Count; i++)
            {
                GameObject newDrawnMount = OutlineUtils.CreateShapeObject($"Mountain_{i}", mountains[i].outline, mountainLevelTextures[mountains[i].elevationLevel + 1], this.transform, Color.black, 0.1f);
                mJSONb.AddMountain(mountains[i]);

                float z = -mountains[i].elevationLevel * 0.1f;
                newDrawnMount.transform.localPosition += Vector3.forward * z;
            }

            IObjectGenerator pog = new PoissonObjectGenerator(MAP_SIZE, MAP_SIZE, 1.5f, 30, 1500, 10, 1);

            List<ObjectData> trees = pog.Generate(null, mountains);

            foreach (var tree in trees)
            {
                GameObject t = Instantiate(treePrefab, new Vector3(tree.position.x, tree.position.y, -0.05f), Quaternion.identity, transform);

                mJSONb.AddObject(tree);
            }

            IObjectGenerator pog2 = new PoissonObjectGenerator(MAP_SIZE, MAP_SIZE, 4f, 30, 1000, 10, 2);

            List<ObjectData> rocks = pog.Generate(null, mountains);

            foreach (var rock in rocks)
            {
                GameObject t = Instantiate(rockPrefab, new Vector3(rock.position.x, rock.position.y, -0.05f), Quaternion.identity, transform);

                mJSONb.AddObject(rock);
            }
        }

        mJSONb.SaveToJson("Assets/Scripts/MapGen/Tests/test_map.json"); */

        StartCoroutine(RunNoiseAlg());
        StartCoroutine(RunSamplingAlg());
    }

    private IEnumerator RunNoiseAlg()
    {
        PerlinNoiseFullAlg pnfa = gameObject.AddComponent<PerlinNoiseFullAlg>();

        pnfa.Initialize(
            0,
            MAP_SIZE,
            MAP_SIZE,
            0
        );

        yield return pnfa.RunAlg();

        Debug.Log("KONIEC");
    }

    private IEnumerator RunSamplingAlg()
    {
        PoissonSamplingFullAlg psfa = gameObject.AddComponent<PoissonSamplingFullAlg>();

        psfa.Initialize(
            0,
            MAP_SIZE,
            0,
            MAP_SIZE,
            4f,
            300,
            20,
            30
        );

        yield return psfa.RunAlg();

        Debug.Log("KONIEC");
    }

    private IEnumerator RunVoronoiAlg()
    {
        Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

        for (int i = 0; i < 20; i++)
        {
            colorMap[i] = GetDistinctColor(i);
        }

        VoronoiDiagramFullAlg vdfa = gameObject.AddComponent<VoronoiDiagramFullAlg>();

        vdfa.Initialize(
            0,
            MAP_SIZE,
            MAP_SIZE,
            0,
            20,
            (a, b) =>
            {
                return Vector2.Distance(a, b);
            },
            colorMap
        );

        yield return vdfa.RunAlg();

        Debug.Log("KONIEC");
    }

    private IEnumerator RunCellularAlg()
    {
        Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

        int min = 0;
        int max = 1;

        Color lowColor = Color.white;
        Color highColor = Color.black;

        for (int i = min; i <= max; i++)
        {
            float t = (float)(i - min) / (max - min); // normalized 0..1
            colorMap[i] = Color.Lerp(lowColor, highColor, t);
        }

        CellularAutomata automataOnFinish;

        CellularAutomataFullAlg cafa = gameObject.AddComponent<CellularAutomataFullAlg>();

        cafa.Initialize(
            0,
            MAP_SIZE,
            MAP_SIZE,
            0,
            2,
            (grid, x, y) =>
            {
                int width = grid.GetLength(0);
                int height = grid.GetLength(1);

                int sum = 0;
                int count = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height && (nx!=x || ny!=y))
                        {
                            sum += grid[nx, ny];
                            count++;
                        }
                    }
                }

                int newValue;

                //Classic cavegen

                if(grid[x,y] == 1)
                {
                    //Wall stays wall if >=4 neighbours are wall
                    newValue = (sum >= 4) ? 1 : 0;
                }
                else
                {
                    //Floor becomes wall if >=5 neighbours are wall
                    newValue = (sum >= 5) ? 1 : 0;
                }

                return newValue;
            },
            colorMap
        );

        yield return cafa.RunAlg();

        automataOnFinish = cafa.GetAutomata();
    }

    private Color GetDistinctColor(int i)
{
    switch (i)
    {
        case 0:  return Color.red;
        case 1:  return Color.green;
        case 2:  return Color.blue;
        case 3:  return Color.yellow;
        case 4:  return Color.cyan;
        case 5:  return Color.magenta;
        case 6:  return new Color(1f, 0.5f, 0f);      // orange
        case 7:  return new Color(0.5f, 0f, 1f);      // purple
        case 8:  return new Color(0f, 0.5f, 1f);      // sky blue
        case 9:  return new Color(0f, 1f, 0.5f);      // turquoise
        case 10: return new Color(0.5f, 1f, 0f);      // lime
        case 11: return new Color(1f, 0f, 0.5f);      // pink-red
        case 12: return new Color(0.6f, 0.3f, 0f);    // brown
        case 13: return new Color(0.3f, 0.3f, 0.3f);  // dark gray
        case 14: return new Color(0.8f, 0.8f, 0.8f);  // light gray
        case 15: return new Color(0f, 0.6f, 0.3f);    // teal green
        case 16: return new Color(0.6f, 0f, 0.3f);    // wine
        case 17: return new Color(0.3f, 0.6f, 0f);    // olive
        case 18: return new Color(0f, 0.3f, 0.6f);    // deep blue
        case 19: return new Color(0.9f, 0.9f, 0.2f);  // pale yellow
        default: return Color.white;
    }
}
}
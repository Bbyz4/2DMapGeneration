using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class OutlineUtils
{
    public static bool IsPointInOutline(Vector2 point, List<Vector2> outline)
    {
        bool inside = false;
        int count = outline.Count;

        for(int i = 0, j = count - 1; i < count; j = i++)
        {
            Vector2 pi = outline[i];
            Vector2 pj = outline[j];

            bool intersect = ((pi.y > point.y) != (pj.y > point.y)) && (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x);

            if(intersect)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    public static bool IsOutlineFullyInside(List<Vector2> innerOutline, List<Vector2> outerOutline)
    {
        foreach(Vector2 p in innerOutline)
        {
            if(!IsPointInOutline(p, outerOutline))
            {
                return false;
            }
        }

        if(DoOutlinesIntersect(innerOutline, outerOutline))
        {
            return false;
        }

        return true;
    }

    private static bool DoOutlinesIntersect(List<Vector2> a, List<Vector2> b)
    {
        for(int i = 0; i < a.Count; i++)
        {
            Vector2 a1 = a[i];
            Vector2 a2 = a[(i + 1) % a.Count];

            for(int j = 0; j < b.Count; j++)
            {
                Vector2 b1 = b[j];
                Vector2 b2 = b[(j + 1) % b.Count];

                if(DoSegmentsIntersect(a1, a2, b1, b2))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool DoSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);

        return o1 != o2 && o3 != o4;
    }

    private static float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float value = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);

        if (Mathf.Abs(value) < 0.0001f)
        {
            return 0;
        }

        return value > 0 ? 1 : 2;
    }

    private static Mesh CreateMeshFromOutline(List<Vector2> outline, Rect textureUvRect)
    {
        if (outline == null || outline.Count < 3)
        {
            Debug.LogError("Outline must have at least 3 points to create a mesh.");
            return null;
        }

        // 1. Setup Vertices
        Vector3[] vertices = new Vector3[outline.Count];
        Vector2[] uvs = new Vector2[outline.Count];
        
        // Find bounds for UV mapping if textureUvRect is not provided
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        foreach(var p in outline) {
            if(p.x < minX) minX = p.x; if(p.x > maxX) maxX = p.x;
            if(p.y < minY) minY = p.y; if(p.y > maxY) maxY = p.y;
        }

        for (int i = 0; i < outline.Count; i++)
        {
            vertices[i] = new Vector3(outline[i].x, outline[i].y, 0);
            
            float tileSize = 1f; // world units per texture tile
            uvs[i] = new Vector2(
                outline[i].x / tileSize,
                outline[i].y / tileSize
            );
        }

        // 2. Triangulation
        // Unity's complex shapes require an algorithm to determine which points form triangles
        Triangulator tr = new Triangulator(outline.ToArray());
        int[] indices = tr.Triangulate();

        // 3. Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public static GameObject CreateShapeObject(string name, List<Vector2> outline, Texture2D shapeTexture, Transform parent, Color? borderColor = null, float borderWidth = 0.05f)
    {
        // 1. Create the new GameObject
        GameObject shapeObj = new GameObject(name);
        
        // 2. Set the position (Optional: child it to this transform to keep the hierarchy clean)
        shapeObj.transform.SetParent(parent);
        shapeObj.transform.localPosition = Vector3.zero;

        // 3. Add required components
        MeshFilter meshFilter = shapeObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = shapeObj.AddComponent<MeshRenderer>();

        // 4. Generate the Mesh
        Mesh generatedMesh = OutlineUtils.CreateMeshFromOutline(outline, new Rect(0, 0, 1, 1));
        meshFilter.mesh = generatedMesh;

        // 5. Setup the Material
        Shader defaultShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (defaultShader == null) defaultShader = Shader.Find("Unlit/Texture");
        if (defaultShader == null) defaultShader = Shader.Find("Standard");

        Material shapeMaterial = new Material(defaultShader);
        
        if (shapeMaterial.HasProperty("_BaseMap"))
            shapeMaterial.SetTexture("_BaseMap", shapeTexture);
        else
            shapeMaterial.mainTexture = shapeTexture;

        meshRenderer.material = shapeMaterial;

        // 6. Optional border
        if (borderColor.HasValue)
        {
            LineRenderer line = shapeObj.AddComponent<LineRenderer>();

            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = outline.Count;

            Vector3[] positions = new Vector3[outline.Count];
            for (int i = 0; i < outline.Count; i++)
            {
                positions[i] = new Vector3(outline[i].x, outline[i].y, -0.01f);
            }

            line.SetPositions(positions);

            line.startWidth = borderWidth;
            line.endWidth = borderWidth;

            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = borderColor.Value;
            line.endColor = borderColor.Value;
        }

        // 7. Polygon collider
        PolygonCollider2D polyCollider = shapeObj.AddComponent<PolygonCollider2D>();
        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, outline.ToArray());

        return shapeObj;
    }

    public static Rect GetBoundingRect(List<Vector2> outline)
    {
        float minX = outline.Min(p => p.x);
        float maxX = outline.Max(p => p.x);
        float minY = outline.Min(p => p.y);
        float maxY = outline.Max(p => p.y);

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    private static int[,] CutOutlineBorderStep(int[,] grid, int width, int height)
    {
        int[,] result = new int[width, height];

        for(int x=1; x<width-1; x++)
        {
            for(int y=1; y<height-1; y++)
            {
                if(grid[x,y] == 1 && grid[x-1,y] == 1 && grid[x+1,y] == 1 && grid[x,y-1] == 1 && grid[x,y+1] == 1)
                {
                    result[x,y] = 1;
                }
            }
        }

        return result;
    }

    //For a given outline and border size, return an outline with the border of given size cut out
    public static List<Vector2> CutOutlineBorder(List<Vector2> outline, int borderSize)
    {
        if(borderSize <= 0)
        {
            return new List<Vector2>(outline);
        }

        Rect bounds = GetBoundingRect(outline);

        int width = Mathf.CeilToInt(bounds.width);
        int height = Mathf.CeilToInt(bounds.height);

        int[,] grid = new int[width, height];

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                Vector2 p = new Vector2(bounds.xMin + x + 0.5f, bounds.yMin + y + 0.5f);
                if(IsPointInOutline(p, outline))
                {
                    grid[x,y] = 1;
                }
            }
        }

        for(int i=0; i<borderSize; i++)
        {
            grid = CutOutlineBorderStep(grid, width, height);
        }

        return GeneratorUtils.BuildMountainOutlines(grid, width, height, (int)bounds.xMin, (int)bounds.yMin).FirstOrDefault().outline;

    }

    //For two given outlines, return their intersection
    public static List<Vector2> GetOutlinesIntersection(List<Vector2> a, List<Vector2> b)
    {
        Rect boundsA = GetBoundingRect(a);
        Rect boundsB = GetBoundingRect(b);

        Rect bounds = Rect.MinMaxRect(
            Mathf.Min(boundsA.xMin, boundsB.xMin),
            Mathf.Min(boundsA.yMin, boundsB.yMin),
            Mathf.Max(boundsA.xMax, boundsB.xMax),
            Mathf.Max(boundsA.yMax, boundsB.yMax)
        );

        int width = Mathf.CeilToInt(bounds.width);
        int height = Mathf.CeilToInt(bounds.height);

        int[,] grid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 p = new Vector2(bounds.xMin + x + 0.5f, bounds.yMin + y + 0.5f);

                if (IsPointInOutline(p, a) && IsPointInOutline(p, b))
                    grid[x, y] = 1;
            }
        }

        List<MountainData> something = GeneratorUtils.BuildMountainOutlines(grid, width, height, (int)bounds.xMin, (int)bounds.yMin);

        if(something.Count == 0)
        {
            return null;
        }
        else
        {
            return something.First().outline;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

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

    public static Mesh CreateMeshFromOutline(List<Vector2> outline, Rect textureUvRect)
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
            
            // Simple UV mapping: maps the shape's bounding box to the 0-1 texture space
            float xNorm = Mathf.InverseLerp(minX, maxX, outline[i].x);
            float yNorm = Mathf.InverseLerp(minY, maxY, outline[i].y);
            uvs[i] = new Vector2(xNorm, yNorm);
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
}

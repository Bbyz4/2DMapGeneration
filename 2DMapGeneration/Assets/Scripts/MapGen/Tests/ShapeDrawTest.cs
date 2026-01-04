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
            // You can now call this multiple times to create multiple shapes
            CreateShapeObject("TestShape", shapeOutline);
        }
    }

    public GameObject CreateShapeObject(string name, List<Vector2> outline)
    {
        // 1. Create the new GameObject
        GameObject shapeObj = new GameObject(name);
        
        // 2. Set the position (Optional: child it to this transform to keep the hierarchy clean)
        shapeObj.transform.SetParent(this.transform);
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

        return shapeObj;
    }
}
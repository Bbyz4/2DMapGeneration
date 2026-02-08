using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseVisualizer : MonoBehaviour
{

    private PerlinNoise NOISE;
    private float VIS_SCALE;
    private Vector2 VIS_MOVEMENT;

    [SerializeField] private GameObject arrowPrefab;

    private List<GameObject> generatedArrows;

    private int left;
    private int right;
    private int up;
    private int down;

    public void Initialize(PerlinNoise noise, float visScale = 1f, float visMovementX = 0f, float visMovementY = 0f)
    {
        this.NOISE = noise;
        this.VIS_SCALE = visScale;
        this.VIS_MOVEMENT = new Vector2(visMovementX, visMovementY);

        this.left = noise.left;
        this.right = noise.right;
        this.up = noise.up;
        this.down = noise.down;

        generatedArrows = new List<GameObject>();
    }

    public IEnumerator VisStep()
    {
        Dictionary<Vector2Int, Vector2> grads = NOISE.GetGradients();

        foreach(var grad in grads)
        {
            float angle = Mathf.Atan2(grad.Value.y, grad.Value.x) * Mathf.Rad2Deg;

            generatedArrows.Add(Instantiate(arrowPrefab, new Vector3(grad.Key.x, grad.Key.y, 0), Quaternion.Euler(new Vector3(0,0,angle))));
        }

        yield return new WaitForSeconds(1f);
    }

    public void Cleanup()
    {
        if (generatedArrows == null)
            return;

        foreach (GameObject go in generatedArrows)
        {
            if (go != null)
                Destroy(go);
        }

        generatedArrows = null;
    }
}
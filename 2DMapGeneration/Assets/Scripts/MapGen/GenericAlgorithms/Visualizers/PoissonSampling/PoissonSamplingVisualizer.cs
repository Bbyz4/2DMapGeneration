using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonSamplingVisualizer : MonoBehaviour
{
    private PoissonSampling SAMPLER;

    private float VIS_SCALE;
    private Vector2 VIS_MOVEMENT;

    [SerializeField] private GameObject pointPrefab;

    private List<GameObject> generatedPoints;

    private float left;
    private float right;
    private float top;
    private float bottom;   

    public void Initialize(PoissonSampling sampler, float visScale = 1f, float visMovementX = 0f, float visMovementY = 0f)
    {
        this.SAMPLER = sampler;
        this.VIS_SCALE = visScale;
        this.VIS_MOVEMENT = new Vector2(visMovementX, visMovementY);

        this.left = sampler.left;
        this.right = sampler.right;
        this.top = sampler.top;
        this.bottom = sampler.bottom;

        generatedPoints = new List<GameObject>();
    }

    public IEnumerator VisStep()
    {
        List<Vector2> points = SAMPLER.GetPreviouslyGeneratedPoints();

        foreach(Vector2 p in points)
        {
            generatedPoints.Add(Instantiate(pointPrefab, new Vector3(p.x, p.y, 0f), Quaternion.identity));
        }

        yield return new WaitForSeconds(1f);
    }

    public void Cleanup()
    {
        if (generatedPoints == null)
            return;

        foreach (GameObject go in generatedPoints)
        {
            if (go != null)
                Destroy(go);
        }

        generatedPoints = null;
    }
}

using System.Collections;
using UnityEngine;

public class PoissonSamplingFullAlg : MonoBehaviour
{
    private PoissonSampling sampling;
    private PoissonSamplingVisualizer samplingVisualizer;

    public void Initialize(
        float left,
        float right,
        float bottom,
        float top,
        float minDistance,
        int maxPoints,
        int rejectionSamples,
        int initialPoints,
        float visScale = 1f,
        float visMovementX = 0f,
        float visMovementY = 0f
    )
    {
        sampling = new PoissonSampling(left, right, bottom, top, minDistance, maxPoints, rejectionSamples, initialPoints);

        samplingVisualizer = gameObject.AddComponent<PoissonSamplingVisualizer>();

        samplingVisualizer.Initialize(sampling, visScale, visMovementX, visMovementY);
    }

    public IEnumerator RunAlg()
    {
        sampling.Generate();
        yield return samplingVisualizer.VisStep();
    }

    public void Cleanup()
    {
        samplingVisualizer.Cleanup();
    }

    public PoissonSampling GetSampling()
    {
        return sampling;
    }

}
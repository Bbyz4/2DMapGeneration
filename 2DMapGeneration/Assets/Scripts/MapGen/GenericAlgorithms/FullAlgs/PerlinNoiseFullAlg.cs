using System.Collections;
using UnityEngine;

public class PerlinNoiseFullAlg : MonoBehaviour
{
    private PerlinNoise noise;
    private PerlinNoiseVisualizer noiseVisualizer;

    public void Initialize(
        int left,
        int right,
        int up,
        int down,
        float visScale = 1f,
        float visMovementX = 0f,
        float visMovementY = 0f
    )
    {
        noise = new PerlinNoise(left, right, up, down);

        noiseVisualizer = gameObject.AddComponent<PerlinNoiseVisualizer>();

        noiseVisualizer.Initialize(noise, visScale, visMovementX, visMovementY);
    }

    public IEnumerator RunAlg()
    {
        noise.Randomize();
        yield return noiseVisualizer.VisStep();
    }

    public void Cleanup()
    {
        noiseVisualizer.Cleanup();
    }

    public PerlinNoise GetNoise()
    {
        return noise;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagramFullAlg : MonoBehaviour
{
    private VoronoiDiagram diagram;
    private VoronoiDiagramVisualizer diagramVisualizer;

    private int randomPointCount;
    private Func<Vector2, Vector2, float> distanceFunction;

    public void Initialize(
        int left,
        int right,
        int up,
        int down,
        int randomPointCount,
        Func<Vector2, Vector2, float> distanceFunction,
        Dictionary<int, Color> colorMap,
        float visScale = 1f,
        float visMovementX = 0f,
        float visMovementY = 0f
    )
    {
        diagram = new VoronoiDiagram(left, right, up, down);

        diagramVisualizer = gameObject.AddComponent<VoronoiDiagramVisualizer>();

        diagramVisualizer.Initialize(diagram, colorMap, visScale, visMovementX, visMovementY);

        this.randomPointCount = randomPointCount;
        this.distanceFunction = distanceFunction;
    }

    public IEnumerator RunAlg()
    {
        diagram.Generate(randomPointCount, distanceFunction);
        yield return diagramVisualizer.VisStep();
    }

    public VoronoiDiagram GetDiagram()
    {
        return diagram;
    }
}
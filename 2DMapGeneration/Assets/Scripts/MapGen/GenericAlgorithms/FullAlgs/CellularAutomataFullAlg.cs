using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataFullAlg : MonoBehaviour
{
    private CellularAutomata automata;
    private CellularAutomataVisualizer automataVisualizer;

    public void Initialize(
        int left,
        int right,
        int up,
        int down,
        int distinctStates,
        Func<int[,], int, int, int> transitionRule,
        Dictionary<int, Color> colorMap,
        float visScale = 1f,
        float visMovementX = 0f,
        float visMovementY = 0f
    )
    {
        automata = new CellularAutomata(left, right, up, down, distinctStates);

        automata.SetTransitionRule(transitionRule);

        automata.Randomize();

        automataVisualizer = gameObject.AddComponent<CellularAutomataVisualizer>();

        automataVisualizer.Initialize(automata, colorMap);
    }

    public IEnumerator RunAlg()
    {
        for (int i = 0; i < 20; i++)
        {
            automata.ApplyTransition(1);
            yield return automataVisualizer.VisStep();
        }
    }

    public CellularAutomata GetAutomata()
    {
        return automata;
    }
}

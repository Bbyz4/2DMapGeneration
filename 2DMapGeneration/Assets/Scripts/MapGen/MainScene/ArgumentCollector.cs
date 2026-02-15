using System;
using System.Reflection;
using UnityEngine;

public class ArgumentCollector : MonoBehaviour
{
    private AlgorithmLauncher algorithmLauncher;

    [SerializeField] private Transform formParent;


    void Awake()
    {
        algorithmLauncher = GameObject.FindWithTag("AlgorithmLauncher").GetComponent<AlgorithmLauncher>();
    }

    private void BuildAlgorithmUI(Type argsType)
    {
        FieldInfo[] fields = argsType.GetFields();

        foreach(var field in fields)
        {
            Type fieldType = field.FieldType;

            if(fieldType == typeof(int))
            {
                
            }
        }
    }

    public void CollectAndLaunchBiomeArgs(int biomeAlgID)
    {
        Type argsType = algorithmLauncher.GetArgsType(biomeAlgID);

        BuildAlgorithmUI(argsType);
    }
}

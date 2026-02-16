using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArgumentCollector : MonoBehaviour
{
    private AlgorithmLauncher algorithmLauncher;

    [SerializeField] private Transform formParent;

    [SerializeField] private GameObject numberInputPrefab;
    [SerializeField] private GameObject booleanInputPrefab;
    [SerializeField] private GameObject submitButtonPrefab;


    private Dictionary<FieldInfo, GameObject> spawnedInputs;
    private GameObject spawnedSubmitButton;

    void Awake()
    {
        algorithmLauncher = GameObject.FindWithTag("AlgorithmLauncher").GetComponent<AlgorithmLauncher>();

        spawnedInputs = new();
    }

    private void ClearUI()
    {
        foreach(var obj in spawnedInputs.Values)
        {
            Destroy(obj);
        }
        spawnedInputs.Clear();

        Destroy(spawnedSubmitButton);
    }

    private void BuildAlgorithmUI(Type argsType, int biomeAlgID)
    {
        ClearUI();

        FieldInfo[] fields = argsType.GetFields();

        float currentY = 0f;

        foreach(var field in fields)
        {
            Type fieldType = field.FieldType;

            GameObject spawned = null;

            Debug.Log(field.FieldType);

            //ugly, add a dictionary later
            if(field.FieldType == typeof(int) || field.FieldType == typeof(float))
            {
                spawned = Instantiate(numberInputPrefab, new Vector3(0f, currentY, 0f), Quaternion.identity, formParent);
            }
            else if(field.FieldType == typeof(bool))
            {
                spawned = Instantiate(booleanInputPrefab, new Vector3(0f, currentY, 0f), Quaternion.identity, formParent);
            }

            if(spawned != null)
            {
                currentY -= 75f; //ugly, move to constants file

                spawned.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = field.Name;
                spawnedInputs.Add(field, spawned);
            } 
        }

        GameObject submit = Instantiate(submitButtonPrefab, new Vector3(0f, currentY, 0f), Quaternion.identity, formParent);
        spawnedSubmitButton = submit;
        submit.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSubmit(argsType, biomeAlgID);
        });
    }

    private void OnSubmit(Type argsType, int biomeAlgID)
    {
        IBiomeGeneratorArgs argsInstance = (IBiomeGeneratorArgs)Activator.CreateInstance(argsType);

        foreach(var pair in spawnedInputs)
        {
            FieldInfo field = pair.Key;
            GameObject uiComponent = pair.Value;

            if (field.FieldType == typeof(int) || field.FieldType == typeof(float))
            {
                var input = uiComponent.GetComponent<TMP_InputField>();
                int value = int.Parse(input.text);
                field.SetValue(argsInstance, value);
            }
            else if (field.FieldType == typeof(bool))
            {
                var toggle = uiComponent.GetComponent<Toggle>();
                field.SetValue(argsInstance, toggle.isOn);
            }
        }

        //here, later we will validate args. So each algorithm will have a method used for validating received data, which will be launched here

        ClearUI();

        algorithmLauncher.LaunchABiomeGenerator(biomeAlgID, argsInstance);
    }

    public void CollectAndLaunchBiomeArgs(int biomeAlgID)
    {
        Type argsType = algorithmLauncher.GetArgsType(biomeAlgID);

        BuildAlgorithmUI(argsType, biomeAlgID);
    }
}

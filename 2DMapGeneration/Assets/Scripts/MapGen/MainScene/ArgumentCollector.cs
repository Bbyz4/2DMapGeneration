using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ArgumentCollectingFormType
{
    BIOME,
    MOUNTAIN,
    OBJECT
};

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

    private void BuildAlgorithmUI(Type argsType, int biomeAlgID, ArgumentCollectingFormType type, BiomeBehaviour biomeBeh)
    {
        ClearUI();

        FieldInfo[] fields = argsType.GetFields();

        float currentX = 600f;
        float currentY = 400f;

        foreach(var field in fields)
        {
            Type fieldType = field.FieldType;

            GameObject spawned = null;

            Debug.Log(field.FieldType);

            //ugly, add a dictionary later
            if(field.FieldType == typeof(int) || field.FieldType == typeof(float))
            {
                spawned = Instantiate(numberInputPrefab, new Vector3(currentX, currentY, 0f), Quaternion.identity, formParent);
            }
            else if(field.FieldType == typeof(bool))
            {
                spawned = Instantiate(booleanInputPrefab, new Vector3(currentX, currentY, 0f), Quaternion.identity, formParent);
            }

            if(spawned != null)
            {
                currentY -= 50f; //ugly, move to constants file

                spawned.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = field.Name;
                spawnedInputs.Add(field, spawned);
            } 
        }

        GameObject submit = Instantiate(submitButtonPrefab, new Vector3(currentX, currentY, 0f), Quaternion.identity, formParent);
        spawnedSubmitButton = submit;
        submit.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSubmit(argsType, biomeAlgID, type, biomeBeh);
        });
    }

    private void CollectArgs(IGeneratorArgs argsInstance)
    {
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
    }

    private void OnSubmit(Type argsType, int biomeAlgID, ArgumentCollectingFormType type, BiomeBehaviour biomeBeh)
    {
        switch(type)
        {
            case ArgumentCollectingFormType.BIOME:
            {
                IBiomeGeneratorArgs argsInstance = (IBiomeGeneratorArgs)Activator.CreateInstance(argsType);

                CollectArgs(argsInstance);

                //here, later we will validate args. So each algorithm will have a method used for validating received data, which will be launched here

                ClearUI();

                algorithmLauncher.LaunchABiomeGenerator(biomeAlgID, argsInstance);

                break;
            }
            case ArgumentCollectingFormType.MOUNTAIN:
            {
                IMountainGeneratorArgs argsInstance = (IMountainGeneratorArgs)Activator.CreateInstance(argsType);

                CollectArgs(argsInstance);

                //here, later we will validate args. So each algorithm will have a method used for validating received data, which will be launched here

                ClearUI();

                algorithmLauncher.LaunchAMountainGenerator(biomeAlgID, argsInstance, biomeBeh);

                break;
            }
            case ArgumentCollectingFormType.OBJECT:
            {
                IObjectGeneratorArgs argsInstance = (IObjectGeneratorArgs)Activator.CreateInstance(argsType);

                CollectArgs(argsInstance);

                //here, later we will validate args. So each algorithm will have a method used for validating received data, which will be launched here

                ClearUI();

                algorithmLauncher.LaunchAnObjectGenerator(biomeAlgID, argsInstance, biomeBeh);

                break;
            }

            default:
                break;
        }
    }

    public void CollectAndLaunchBiomeArgs(int biomeAlgID)
    {
        Type argsType = algorithmLauncher.GetBiomeArgsType(biomeAlgID);

        BuildAlgorithmUI(argsType, biomeAlgID, ArgumentCollectingFormType.BIOME, null);
    }

    public void CollectAndLaunchMountainArgs(int mountainAlgID, BiomeBehaviour biomeBeh)
    {
        Type argsType = algorithmLauncher.GetMountainArgsType(mountainAlgID);

        BuildAlgorithmUI(argsType, mountainAlgID, ArgumentCollectingFormType.MOUNTAIN, biomeBeh);
    }

    public void CollectAndLaunchObjectArgs(int objectAlgID, BiomeBehaviour biomeBeh)
    {
        Type argsType = algorithmLauncher.GetObjectArgsType(objectAlgID);

        BuildAlgorithmUI(argsType, objectAlgID, ArgumentCollectingFormType.OBJECT, biomeBeh);
    }
}

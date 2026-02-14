using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGeneratorDescriptor
{
    public Type generatorType;
    public Type argsType;

    public BiomeGeneratorDescriptor(Type generatorType, Type argsType)
    {
        this.generatorType = generatorType;
        this.argsType = argsType;
    }
}

public class AlgorithmLauncher : MonoBehaviour
{
    private static Dictionary<int, BiomeGeneratorDescriptor> BIOME_GENERATORS = new Dictionary<int, BiomeGeneratorDescriptor>
    {
        {0, new BiomeGeneratorDescriptor(typeof(VoronoiIslandBiomeGenerator),typeof(VoronoiIslandBiomeGeneratorArgs)
    )}
    };

    private static IBiomeGenerator CreateBiomeGenInstance(int ID, GameObject parentObj)
    {
        if(!BIOME_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return (IBiomeGenerator)parentObj.AddComponent(BIOME_GENERATORS[ID].generatorType);
    }

    public Type GetArgsType(int ID)
    {
        if(!BIOME_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return BIOME_GENERATORS[ID].argsType;
    }

    public void LaunchABiomeGenerator(int ID, IBiomeGeneratorArgs args)
    {
        IBiomeGenerator generator = CreateBiomeGenInstance(ID, gameObject);  

        Vector2Int mapSize = GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().MAP_SIZE;

        //do something with arguments, create a new interface for alg arguments

        generator.Initialize(args);
        List<BiomeData> result =  generator.Generate(mapSize);
    
        GameObject.FindWithTag("UILoader").GetComponent<UILoader>().HideBiomeGeneratorPopup();    
        GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().PlaceBiomes(result);

        Destroy((MonoBehaviour)generator);
    }
}

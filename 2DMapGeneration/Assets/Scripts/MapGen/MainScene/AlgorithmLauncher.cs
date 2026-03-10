using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorDescriptor
{
    public Type generatorType;
    public Type argsType;

    public GeneratorDescriptor(Type generatorType, Type argsType)
    {
        this.generatorType = generatorType;
        this.argsType = argsType;
    }
}

public class AlgorithmLauncher : MonoBehaviour
{
    private static Dictionary<int, GeneratorDescriptor> BIOME_GENERATORS = new Dictionary<int, GeneratorDescriptor>
    {
        {0, new GeneratorDescriptor(typeof(VoronoiIslandBiomeGenerator),typeof(VoronoiIslandBiomeGeneratorArgs))}
    };

    private static Dictionary<int, GeneratorDescriptor> MOUNTAIN_GENERATORS = new Dictionary<int, GeneratorDescriptor>
    {
        {0, new GeneratorDescriptor(typeof(PerlinMountainGenerator), typeof(PerlinMountainGeneratorArgs))},
        {1, new GeneratorDescriptor(typeof(WFCMountainGenerator), typeof(WFCMountainGeneratorArgs))},
        {2, new GeneratorDescriptor(typeof(WeierstrassMountainGenerator), typeof(WeierstrassMountainGeneratorArgs))}
    };

    private static Dictionary<int, GeneratorDescriptor> OBJECT_GENERATORS = new Dictionary<int, GeneratorDescriptor>
    {
        {0, new GeneratorDescriptor(typeof(PoissonObjectGenerator), typeof(PoissonObjectGeneratorArgs))}
    };

    private static IBiomeGenerator CreateBiomeGenInstance(int ID, GameObject parentObj)
    {
        if(!BIOME_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return (IBiomeGenerator)parentObj.AddComponent(BIOME_GENERATORS[ID].generatorType);
    }

    private static IMountainGenerator CreateMountainGenInstance(int ID, GameObject parentObj)
    {
        if(!MOUNTAIN_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!"); 
        }

        return (IMountainGenerator)parentObj.AddComponent(MOUNTAIN_GENERATORS[ID].generatorType);
    }

    private static IObjectGenerator CreateObjectGenInstance(int ID, GameObject parentObj)
    {
        if(!OBJECT_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!"); 
        }

        return (IObjectGenerator)parentObj.AddComponent(OBJECT_GENERATORS[ID].generatorType);
    }

    public Type GetBiomeArgsType(int ID)
    {
        if(!BIOME_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return BIOME_GENERATORS[ID].argsType;
    }

    public Type GetMountainArgsType(int ID)
    {
        if(!MOUNTAIN_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return MOUNTAIN_GENERATORS[ID].argsType;
    }

    public Type GetObjectArgsType(int ID)
    {
        if(!OBJECT_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return OBJECT_GENERATORS[ID].argsType;
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

    public void LaunchAMountainGenerator(int ID, IMountainGeneratorArgs args, BiomeBehaviour biome)
    {
        IMountainGenerator generator = CreateMountainGenInstance(ID, gameObject);

        generator.Initialize(args);
        List<MountainData> result = generator.Generate(biome.GetData());

        GameObject.FindWithTag("UILoader").GetComponent<UILoader>().HideMountainGeneratorPopup();    
        GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().PlaceMountains(result, biome);

        Destroy((MonoBehaviour)generator);
    }

    public void LaunchAnObjectGenerator(int ID, IObjectGeneratorArgs args, BiomeBehaviour biome, int generatedObjectID)
    {
        IObjectGenerator generator = CreateObjectGenInstance(ID, gameObject);

        generator.Initialize(args);
        List<ObjectData> result = generator.Generate(biome.GetData(), (List<MountainData>)biome.GetMountainDatas(), generatedObjectID);

        GameObject.FindWithTag("UILoader").GetComponent<UILoader>().HideObjectGeneratorPopup();    
        GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().PlaceObjects(result, biome);

        Destroy((MonoBehaviour)generator);
    }
}

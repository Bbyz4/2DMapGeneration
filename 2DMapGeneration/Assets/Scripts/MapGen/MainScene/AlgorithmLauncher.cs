using System;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmLauncher : MonoBehaviour
{
    private static Dictionary<int, Type> BIOME_GENERATORS = new Dictionary<int, Type>
    {
        {0, typeof(VoronoiIslandBiomeGenerator)}
    };

    private static IBiomeGenerator CreateBiomeGenInstance(int ID, GameObject parentObj)
    {
        if(!BIOME_GENERATORS.ContainsKey(ID))
        {
            throw new Exception($"Generator with ID {ID} not found!");
        }

        return (IBiomeGenerator)parentObj.AddComponent(BIOME_GENERATORS[ID]);
    }



    public void LaunchABiomeGenerator(int ID)
    {
        IBiomeGenerator generator = CreateBiomeGenInstance(ID, gameObject);  

        Vector2Int mapSize = GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().MAP_SIZE;

        //temporary, very ugly, should not be here
        generator = new VoronoiIslandBiomeGenerator(mapSize.x, mapSize.y, 10, 100, true);

        //do something with arguments, create a new interface for alg arguments

        List<BiomeData> result =  generator.Generate(mapSize);
    
        GameObject.FindWithTag("UILoader").GetComponent<UILoader>().HideBiomeGeneratorPopup();    
        GameObject.FindWithTag("ObjectPlacer").GetComponent<ObjectPlacer>().PlaceBiomes(result);

        Destroy((MonoBehaviour)generator);
    }
}

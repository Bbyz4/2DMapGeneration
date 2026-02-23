using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeCharacteristics
{
    [SerializeField] private string biomeName;
    [SerializeField] private Texture2D defaultTurf;
    [SerializeField] private Texture2D mountainLevel1Turf;
    [SerializeField] private Texture2D mountainLevel2Turf;
    [SerializeField] private Texture2D mountainLevel3Turf;
    [SerializeField] private Texture2D lakeTurf;

    public string BiomeName => biomeName;

    public Texture2D GetDefaultTurf()
    {
        return defaultTurf;
    }

    public Texture2D GetMountainTurf(int level)
    {
        switch(level)
        {
            case -1:
                return this.GetLakeTurf();
            case 1:
                return mountainLevel1Turf;
            case 2:
                return mountainLevel2Turf;
            case 3:
                return mountainLevel3Turf;
            default:
                return null;
        }
    }

    public Texture2D GetLakeTurf()
    {
        return lakeTurf;
    }
}

public class BiomeTypeManager : MonoBehaviour
{
    [SerializeField] private List<BiomeCharacteristics> biomes;

    public BiomeCharacteristics GetBiomeCharacteristicsFromID(int biomeID)
    {
        if(biomeID < 0 || biomeID >= biomes.Count)
        {
            return null;
        }        

        return biomes[biomeID];
    }
}
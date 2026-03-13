using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BiomeCharacteristics
{
    [SerializeField] private string biomeName;
    [SerializeField] private TileBase defaultTurf;
    [SerializeField] private TileBase mountainLevel1Turf;
    [SerializeField] private TileBase mountainLevel2Turf;
    [SerializeField] private TileBase mountainLevel3Turf;
    [SerializeField] private TileBase lakeTurf;

    [SerializeField] private Texture2D baseTexture;

    public string BiomeName => biomeName;

    public TileBase GetDefaultTurf()
    {
        return defaultTurf;
    }

    public TileBase GetMountainTurf(int level)
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

    public TileBase GetLakeTurf()
    {
        return lakeTurf;
    }

    public Texture2D GetBaseTexture()
    {
        return baseTexture;
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
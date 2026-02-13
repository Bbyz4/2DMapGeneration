using UnityEngine;

public class UILoader : MonoBehaviour
{
    [SerializeField] private BiomeGeneratorPopup biomeGeneratorPopup;
    [SerializeField] private MountainGeneratorPopup mountainGeneratorPopup;
    [SerializeField] private ObjectGeneratorPopup objectGeneratorPopup;

    void Awake()
    {
        biomeGeneratorPopup.gameObject.SetActive(true);
    }
}

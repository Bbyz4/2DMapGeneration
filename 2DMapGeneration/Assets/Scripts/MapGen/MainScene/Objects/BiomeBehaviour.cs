using UnityEngine;

public class BiomeBehaviour : MonoBehaviour
{
    private GameObject UILoader;
    void Awake()
    {
        UILoader = GameObject.FindWithTag("UILoader");

        gameObject.AddComponent<PolygonCollider2D>();
    }

    void OnMouseDown()
    {
        Debug.Log("Biome click");

        UILoader.GetComponent<UILoader>().LoadBiomeChooseTypePopup(gameObject);       
    }
}

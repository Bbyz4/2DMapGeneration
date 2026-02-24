using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    void Awake()
    {
        CameraPanZoom2D.OnZoomChanged += OnZoomChange;
        OnZoomChange(GameObject.FindWithTag("MainCamera").GetComponent<CameraPanZoom2D>().GetCurrentKeyZoom());
    }

    private void OnZoomChange(float newZoom)
    {
        gameObject.transform.localScale = new Vector3(f(newZoom), f(newZoom), 1f);
    }

    private float f(float x)
    {
        return 0.0918367f*x + 0.816327f;
    }
}
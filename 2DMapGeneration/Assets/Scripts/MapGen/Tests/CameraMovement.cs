using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPanZoom2D : MonoBehaviour
{
    public float panSpeed = 1.0f;
    public float zoomSpeed = 10f;
    public float minZoom = 2f;
    public float maxZoom = 50f;

    private Camera cam;
    private Vector3 lastMouseWorldPos;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        cam.orthographicSize = maxZoom;
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = lastMouseWorldPos - currentMouseWorldPos;

            transform.position += delta;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.0001f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}

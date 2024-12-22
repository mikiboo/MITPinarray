using UnityEngine;

public class UICameraControl : MonoBehaviour
{
    public RectTransform uiElement; // Assign your UI element here

    private Vector3 lastMousePosition;
    private bool isDragging;

    public float dragSpeed = 1.0f; // Adjust the drag speed
    public float zoomSpeed = 0.1f; // Adjust the scale speed
    public float minScale = 0.5f; // Minimum scale factor
    public float maxScale = 2.0f; // Maximum scale factor

    void Update()
    {
        // Dragging
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }

        if (isDragging && Input.GetMouseButton(2))
        {
            Vector3 delta = (Input.mousePosition - lastMousePosition) * dragSpeed;
            Vector3 newPosition = uiElement.position + delta;
            uiElement.position = newPosition;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        // Zooming (Scaling)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 newScale = uiElement.localScale + Vector3.one * scroll * zoomSpeed;
        newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
        newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale); // In case of 3D scaling requirements
        uiElement.localScale = newScale;
    }
}

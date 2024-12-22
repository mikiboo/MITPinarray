using UnityEngine;

public class MouseCameraControl : MonoBehaviour
{
    public Camera camera; // You might still reference the camera for calculations, but not to move it.
    public Transform objectToMove; // This is the object you want to move and scale.

    public float rotationSpeed = 300.0f; // Rotation speed
    public float dragSpeed = 2.0f; // Drag speed to move the object

    private Vector3 lastMousePosition;
    private bool isDragging; // Renamed from isPanning for clarity

    public float zoomSpeed = 20f; // Adjusted zoom speed for smoother zoom
    public float minZoom = 5f; // Minimum zoom value
    public float maxZoom = 80f; // Maximum zoom value

    void Update()
    {
        // Rotation with right mouse button
        if (Input.GetMouseButton(1))
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            objectToMove.Rotate(Vector3.up, -rotationX, Space.World);
            objectToMove.Rotate(Vector3.right, rotationY, Space.World);
        }

        // Dragging with middle mouse button to update object location
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(2))
        {
            if (!isDragging)
            {
                if (Vector3.Distance(lastMousePosition, Input.mousePosition) > Mathf.Epsilon)
                {
                    isDragging = true;
                }
            }
            else
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 move = new Vector3(delta.x * dragSpeed, delta.y * dragSpeed, 0) * Time.deltaTime;
                objectToMove.position += camera.ScreenToWorldPoint(move) - camera.ScreenToWorldPoint(Vector3.zero);
                lastMousePosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (camera.orthographic)
        {
            camera.orthographicSize -= scroll * zoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            camera.fieldOfView -= scroll * zoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minZoom, maxZoom);
        }
    }
}

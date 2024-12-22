using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using USPinTable;

public class BoxSelector : MonoBehaviour
{
    public PinTableGenerator pinTableGenerator;

    public RectTransform selectionBox;
    public Canvas canvas;
    private Vector2 startPosition;
    private Rect selectionRect;

    void Update()
    {
        // Detect if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        // While dragging the mouse
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        // If the left mouse button is released, end selection
        if (Input.GetMouseButtonUp(0))
        {
            SelectObjects();
            selectionBox.gameObject.SetActive(false);
        }
    }

    void UpdateSelectionBox(Vector2 currentMousePosition)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        Vector2 localStartPoint;
        Vector2 localCurrentPoint;

        // Convert screen point to local point in rectangle space
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)selectionBox.parent, startPosition,
            canvas.worldCamera, out localStartPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)selectionBox.parent,
            currentMousePosition, canvas.worldCamera, out localCurrentPoint);

        // Calculate the size and position of the rectangle
        float width = localCurrentPoint.x - localStartPoint.x;
        float height = localCurrentPoint.y - localStartPoint.y;

        selectionRect = new Rect(localStartPoint.x, localStartPoint.y, width, height);

        // Adjust the size and position of the selection box
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = localStartPoint + new Vector2(width / 2, height / 2);
    }

    public void SelectObjects()
    {
        Vector3[] selectionCorners = new Vector3[4];
        selectionBox.GetWorldCorners(selectionCorners); // Populate with the corners of the selection box
        
        for (var i = 0; i < pinTableGenerator.rows; i++)
        {
            for (var j = 0; j < pinTableGenerator.columns; j++)
            {
                Vector3[] pinCorners = new Vector3[4];
                pinTableGenerator.PinTable[i, j].image.rectTransform
                    .GetWorldCorners(pinCorners); // Populate with the corners of the pin

                // Check if any corner of the pin is within the selection box
                if (IsPinWithinSelection(pinCorners, selectionCorners))
                {
                    pinTableGenerator.OnPinClicked(i,j); // Simulate a click on the pin
                }
            }
        }
    }

    bool IsPinWithinSelection(Vector3[] pinCorners, Vector3[] selectionCorners)
    {
        // Example check: This function should be implemented to check if any of the pin's corners
        // fall within the bounds defined by the selectionCorners.
        // A simple way is to check if pinCorners are within the min and max of selectionCorners,
        // considering x and y coordinates.

        // Get min and max bounds of selection
        Vector2 minSelection = new Vector2(selectionCorners.Min(corner => corner.x),
            selectionCorners.Min(corner => corner.y));
        Vector2 maxSelection = new Vector2(selectionCorners.Max(corner => corner.x),
            selectionCorners.Max(corner => corner.y));

        // Check each corner of the pin
        foreach (Vector3 corner in pinCorners)
        {
            if (corner.x >= minSelection.x && corner.x <= maxSelection.x && corner.y >= minSelection.y &&
                corner.y <= maxSelection.y)
            {
                return true; // Pin is within selection
            }
        }

        return false; // Pin is not within selection
    }
}

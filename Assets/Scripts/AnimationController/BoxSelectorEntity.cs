using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoxSelectorEntity : MonoBehaviour
{
    public UIC_Manager uIC_Manager;
    public RectTransform selectionBox;
    public Canvas canvas;
    private Vector2 startPosition;
    private Rect selectionRect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectEntities();
            selectionBox.gameObject.SetActive(false);
        }
    }

    void UpdateSelectionBox(Vector2 currentMousePosition)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        Vector2 localStartPoint;
        Vector2 localCurrentPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)selectionBox.parent, startPosition,
            canvas.worldCamera, out localStartPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)selectionBox.parent,
            currentMousePosition, canvas.worldCamera, out localCurrentPoint);

        float width = localCurrentPoint.x - localStartPoint.x;
        float height = localCurrentPoint.y - localStartPoint.y;

        selectionRect = new Rect(localStartPoint.x, localStartPoint.y, width, height);

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = localStartPoint + new Vector2(width / 2, height / 2);
    }

    void SelectEntities()
    {
        if (uIC_Manager == null || uIC_Manager.EntityList == null)
            return;

        foreach (UIC_Entity entity in uIC_Manager.EntityList)
        {
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, entity.transform.position);
            Debug.Log($"Screen point: {screenPoint}");
            Debug.Log($"entity: {entity.name}");
            if (selectionRect.Contains(screenPoint, true))
            {
                Debug.Log($"kaaaaa");
                uIC_Manager.selectedUIObjectsList.Add(entity);
            }
        }
    }
}

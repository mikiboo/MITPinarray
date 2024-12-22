using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceGraph : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int pointsCount = 100; // Number of points in the graph
    public float updateInterval = 0.1f; // Time interval between updates

    private List<float> values = new List<float>();
    private float timeSinceLastUpdate = 0f;

    void Start()
    {
        // Initialize the values list with random values
        for (int i = 0; i < pointsCount; i++)
        {
            values.Add(Random.Range(0f, 1f));
        }

        // Initialize the line renderer
        lineRenderer.positionCount = pointsCount;
        lineRenderer.startWidth = 0.1f; // Set the width of the line
        lineRenderer.endWidth = 0.1f; // Set the width of the line

        // Assign a material to the line renderer
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        UpdateLineRenderer();
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0f;

            // Generate a new random value and add it to the list
            float newValue = Random.Range(0f, 1f);
            values.Add(newValue);

            // Remove the oldest value to maintain the points count
            if (values.Count > pointsCount)
            {
                values.RemoveAt(0);
            }

            // Update the line renderer
            UpdateLineRenderer();
        }
    }

    void UpdateLineRenderer()
    {
        float width = lineRenderer.transform.parent.GetComponent<RectTransform>().rect.width;
        float height = lineRenderer.transform.parent.GetComponent<RectTransform>().rect.height;
        float xSpacing = width / (pointsCount - 1);

        for (int i = 0; i < pointsCount; i++)
        {
            float x = i * xSpacing;
            float y = values[i] * height;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0)); // Ensure Z is 0
        }
    }
}

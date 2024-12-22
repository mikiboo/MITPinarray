using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCube : MonoBehaviour
{
    public float scale = 10;
    public Color colorMax = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    public Color colorMin = new Color(255f / 255f, 255f / 255f, 255f / 255f); // Ensure opacity is full (alpha = 1)

    private MeshRenderer meshRenderer;
    private int currentHeight;
    private void Awake()
    {
        // Get the MeshRenderer component to change the color
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("No MeshRenderer found on the SingleCube!");
        }
    }

    public void UpdateHeight(int height)
    {
        float previousHeight = transform.localScale.y;
        float newHeight = height / scale + 0.1f;
        transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

        // Adjust position based on the cube's local up direction and the change in height
        transform.position += transform.up * (newHeight - previousHeight) / 2f;
        currentHeight = height;
        // Update the color based on the height
        UpdateCubeColor(height);
    }

    public void UpdateCubeColor(int height)
    {
        // Debug.Log($"height: {height}");
        float fraction = height / 351f;
        // Debug.Log($"fraction {fraction}");
        Color interpolatedColor = Color.Lerp(colorMin, colorMax, fraction);
        // Debug.Log($"cube: {interpolatedColor}");
        if (meshRenderer != null)
        {
            meshRenderer.material.color = interpolatedColor;
        }
    }
    private void OnEnable()
    {
        // Update the color whenever the object is reactivated
        UpdateCubeColor(currentHeight);
    }
    public void UpdateColorRange(Color newColorMin, Color newColorMax)
    {
        colorMin = newColorMin;
        colorMax = newColorMax;
        // Apply the updated color immediately
        UpdateCubeColor((int)(transform.localScale.y * scale));
    }
}

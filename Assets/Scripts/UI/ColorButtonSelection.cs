using UnityEngine;
using UnityEngine.UI;
using USPinTable;  // Import this to work with UI components like Button and Image

public class ColorButtonSelection : MonoBehaviour
{
    public Image targetImage;  // Assign this in the inspector
    public PinTableGenerator pinTableGenerator;
    void Start()
    {
        InitRandomColor();  // Set a random color when the script starts
    }

    public void LogColor()
    {
        if (targetImage != null)
        {
            Color imgColor = targetImage.color;

            // Create a new color with opacity 0
            Color imgColorWithZeroOpacity = new Color(imgColor.r, imgColor.g, imgColor.b, 1f);

            // Create a new color with full opacity (1.0f)
            Color imgColorWithFullOpacity = new Color(imgColor.r, imgColor.g, imgColor.b, 1f);

            // Convert the color to HEX (optional, just for debugging)
            string hexColor = ColorUtility.ToHtmlStringRGB(imgColorWithFullOpacity);
            Debug.Log($"Color of the image in HEX with full opacity: #{hexColor}");

            // Update the color range with the new colors
            pinTableGenerator.UpdateColorMaxMin(imgColorWithZeroOpacity, imgColorWithFullOpacity);

        }
        else
        {
            Debug.Log("Target image not assigned.");
        }
    }

    private void InitRandomColor()
    {
        if (targetImage != null)
        {
            // Generate random color components
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            float a = 1f;  // Fully opaque

            // Apply the random color to the target image
            targetImage.color = new Color(r, g, b, a);
        }
        else
        {
            Debug.Log("Target image not assigned for random color initialization.");
        }
    }
}

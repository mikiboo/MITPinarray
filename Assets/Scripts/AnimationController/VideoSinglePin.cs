using System.Collections;
using UnityEngine;

public class VideoSinglePin : MonoBehaviour
{
    public int height = 0; // 0~300
    public SingleCube cube;
    private float maxHeight = 350f;
    private int maxHeightInt;
    private bool isTransitioning = false;
    public Color colorMax = new(255f / 255f, 106f / 255f, 0f / 255f);
    public Color colorMin = new(255f / 255f, 217f / 255f, 190f / 255f);
    private void Start()
    {

        maxHeightInt = (int)maxHeight;
        // Debug.Log("VideoSinglePin started.");
    }

    public void Initialize(SingleCube cube)
    {
        this.cube = cube;
        maxHeightInt = (int)maxHeight;
        Debug.Log("VideoSinglePin initialized with cube.");
    }

    public void StartHeightTransition(int targetHeight, float waitTime)
    {
        StartCoroutine(SmoothHeightTransition(targetHeight, waitTime));
    }

    private IEnumerator SmoothHeightTransition(int targetHeight, float waitTime)
    {
        isTransitioning = true;
        int startHeight = 0;
        float duration = 5f;
        float elapsedTime = 0f;

        // Transition to target height
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            int currentHeight = (int)Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            // Debug.Log($"Increasing height: {currentHeight}");
            cube.UpdateHeight(currentHeight);
            yield return null;
        }

        // Ensure the final height is set
        cube.UpdateHeight(targetHeight);
        // Debug.Log($"height: {height}");
        // cube.UpdateCubeColor(height);
        waitTime = waitTime == 0 ? 1 : waitTime;
        // Wait for 1 second at the target height
        Debug.Log("Waiting for " + waitTime + " seconds.");
        yield return new WaitForSeconds(waitTime);

        // Reset elapsed time for decreasing height
        elapsedTime = 0f;

        // Transition back to 0 height
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            int currentHeight = (int)Mathf.Lerp(targetHeight, startHeight, elapsedTime / duration);
            // Debug.Log($"Decreasing height: {currentHeight}");
            cube.UpdateHeight(currentHeight);
            yield return null;
        }

        // Ensure the final height is set to 0
        cube.UpdateHeight(startHeight);
        isTransitioning = false;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    public void UpdateColorRange(Color newColorMin, Color newColorMax)
    {
        colorMin = newColorMin;
        colorMax = newColorMax;
        // UpdatePinColor();  // Update the color immediately based on the current height

        if (cube != null)
        {

            cube.UpdateColorRange(newColorMin, newColorMax); // Update the color range for the cube as well
        }
    }
    
}

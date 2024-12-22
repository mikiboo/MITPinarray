using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public Slider loadingSlider;
    public float loadingTime = 5f; // Duration in seconds over which the bar should fully load

    private float currentValue = 0f;

    private bool loadScene = false;

    void Start()
    {
        if (loadingSlider == null)
        {
            loadingSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (currentValue < 1f) // Check if the bar is not yet full
        {
            currentValue += Time.deltaTime / loadingTime; // Increment the value over time
            loadingSlider.value = currentValue; // Apply the new value to the slider
        }

        if (currentValue >= 1f && !loadScene)
        {
            SceneManager.LoadScene("ModeScene");
        }
    }
}

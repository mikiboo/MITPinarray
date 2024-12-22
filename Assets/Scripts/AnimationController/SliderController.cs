using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider loadingSlider;
    public TMP_Text countdownText;  // Reference to the Text component
    public UIC_Manager uIC_Manager;

    void Start()
    {
        if (loadingSlider == null)
        {
            loadingSlider = GetComponent<Slider>();
        }

        if (countdownText == null)
        {
            Debug.LogError("Countdown Text is not assigned.");
        }
    }

    void Update()
    {
        if (uIC_Manager.currentValue < 1f) // Check if the bar is not yet full
        {
            // Increment the value over time
            uIC_Manager.currentValue += Time.deltaTime / uIC_Manager.animationDuration;
            loadingSlider.value = uIC_Manager.currentValue; // Apply the new value to the slider

            // Calculate remaining time and update the text
            int remainingTime = Mathf.CeilToInt((1f - uIC_Manager.currentValue) * uIC_Manager.animationDuration);
            countdownText.text = remainingTime.ToString() + "s";
        }
        else
        {
            // Optionally, hide or disable the text when the countdown is complete
            countdownText.text = "0s";
        }
    }
}

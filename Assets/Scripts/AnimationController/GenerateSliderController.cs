using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenerateSliderController : MonoBehaviour
{
    public Slider loadingSlider;
    public UIC_Manager uIC_Manager;



    void Start()
    {
        if (loadingSlider == null)
        {
            loadingSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (uIC_Manager.GenerateCurrentValue < 1f) // Check if the bar is not yet full
        {
            uIC_Manager.GenerateCurrentValue += Time.deltaTime / uIC_Manager.GenerateAnimationDuration; // Increment the value over time
            loadingSlider.value = uIC_Manager.GenerateCurrentValue; // Apply the new value to the slider
        }

    }
}

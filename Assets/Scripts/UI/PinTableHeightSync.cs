using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using USPinTable;

public class PinTableHeightSync : MonoBehaviour
{
    public PinTableGenerator pinTableGenerator;

    public Slider slider;
    public TMP_InputField inputField;

    private void Start()
    {
        // Subscribe to events
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        inputField.onEndEdit.AddListener(OnInputFieldValueChanged);

        // Initial sync
        inputField.text = slider.value.ToString();
    }

    private void OnSliderValueChanged(float value)
    {
        inputField.text = value.ToString();
        pinTableGenerator.UpdateHeight(inputField.text);
    }

    private void OnInputFieldValueChanged(string text)
    {
        // Update slider value if the input field value is a valid float
        if (float.TryParse(text, out float value))
        {
            slider.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        }
        else
        {
            // If the input is not a valid float, revert to the slider's current value
            inputField.text = slider.value.ToString();
        }
    }
}

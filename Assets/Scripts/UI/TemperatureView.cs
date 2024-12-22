using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureView : MonoBehaviour
{
    public GameObject temperatureSelected;
    public GameObject temperatureUnselected;
    public GameObject selectAllPin;
    public GameObject unselectAllPin;
    public GameObject toogleView;
    public ToggleView toogleViewScript;

    public Slider slider;

    public UICameraControl controlScript;
    public GameObject pinTableTemperaturePanel;
    // Start is called before the first frame update
    void Start()
    {
        temperatureSelected.SetActive(false);
        temperatureUnselected.SetActive(true);
        pinTableTemperaturePanel.SetActive(false);
        controlScript.enabled = false;
    }

    // Update is called once per frame
    public void TemperatureSelected()
    {
        selectAllPin.GetComponent<Button>().interactable = false;
        unselectAllPin.GetComponent<Button>().interactable = false;
        toogleView.GetComponent<Button>().interactable = false;
        toogleViewScript.TurnOffToogleView();
        temperatureSelected.SetActive(true);
        temperatureUnselected.SetActive(false);
        pinTableTemperaturePanel.SetActive(true);
        controlScript.enabled = true;
        slider.interactable = false;
    }

    public void TemperatureUnselected()
    {
        selectAllPin.GetComponent<Button>().interactable = true;
        unselectAllPin.GetComponent<Button>().interactable = true;
        toogleView.GetComponent<Button>().interactable = true;
        toogleViewScript.TurnOnToogleView();
        temperatureSelected.SetActive(false);
        temperatureUnselected.SetActive(true);
        pinTableTemperaturePanel.SetActive(false);
        controlScript.enabled = false;
        slider.interactable = true;
    }
    void Update()
    {

    }
}

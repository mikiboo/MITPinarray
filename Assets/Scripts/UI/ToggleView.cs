using UnityEngine;

public class ToggleView : MonoBehaviour
{
    public GameObject pinTablePanel;
    public GameObject cubeTable3D;
    public MouseCameraControl cameraControlScript;
    public UICameraControl uiControlScript;
    public bool pinTablePanelState = false;
    public bool cubeTable3DState = false;
    public bool cameraControlScriptState = false;
    public bool uiControlScriptState = false;


    public static ToggleView instance;
    private bool is3DView = true;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ViewToggle();
    }
    public void TurnOffToogleView()
    {

        pinTablePanel.SetActive(false);
        cubeTable3D.SetActive(false); // Hide the 3D cube table
        cameraControlScript.enabled = false; // Disable camera control script
        uiControlScript.enabled = false;
    }
    public void TurnOnToogleView()
    {
        pinTablePanel.SetActive(pinTablePanelState);
        cubeTable3D.SetActive(cubeTable3DState); // Hide the 3D cube table
        cameraControlScript.enabled = cameraControlScriptState; // Disable camera control script
        uiControlScript.enabled = uiControlScriptState;
    }
    public void ViewToggle()
    {


        if (is3DView)
        {
            pinTablePanelState = true;
            cubeTable3DState = false;
            cameraControlScriptState = false;
            uiControlScriptState = true;
            pinTablePanel.SetActive(true);
            cubeTable3D.SetActive(false); // Hide the 3D cube table
            cameraControlScript.enabled = false; // Disable camera control script
            uiControlScript.enabled = true;
        }
        else
        {
            pinTablePanelState = false;
            cubeTable3DState = true;
            cameraControlScriptState = true;
            uiControlScriptState = true;
            pinTablePanel.SetActive(false);
            cubeTable3D.SetActive(true); // Show the 3D cube table
            cameraControlScript.enabled = true; // Enable camera control script
            uiControlScript.enabled = false;
        }

        is3DView = !is3DView; // Toggle the state
    }
}

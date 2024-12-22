using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Include for EventTrigger
using USPinTable;

public class CameraAdjuster : MonoBehaviour
{
    public Camera orthographicCamera;
    public Transform parentObject;
    public float padding = 1.0f;
    public VideoPinTableGenerator videoPinTableGenerator;
    private bool isRotate = true;
    private bool zoomingIn = false;
    private bool zoomingOut = false;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button ZoomIn;
    public Button ZoomOut;
    public Button btnRotate;
    public int speed;

    public float zoomSensitivity = 5.0f;

    private void Start()
    {
        btn1.onClick.AddListener(() => buttonActive(0));
        btn2.onClick.AddListener(() => buttonActive(90));
        btn3.onClick.AddListener(() => buttonActive(180));
        btn4.onClick.AddListener(() => buttonActive(270));
        btnRotate.onClick.AddListener(buttonRotateActive);

        AddEventTrigger(ZoomIn, EventTriggerType.PointerDown, () => zoomingIn = true);
        AddEventTrigger(ZoomIn, EventTriggerType.PointerUp, () => zoomingIn = false);
        AddEventTrigger(ZoomOut, EventTriggerType.PointerDown, () => zoomingOut = true);
        AddEventTrigger(ZoomOut, EventTriggerType.PointerUp, () => zoomingOut = false);
    }

    private void AddEventTrigger(Button button, EventTriggerType type, Action action)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }

    private void buttonActive(int angle)
    {
        isRotate = false;
        Vector3 currentRotation = parentObject.eulerAngles;
        parentObject.eulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);
    }

    private void buttonRotateActive()
    {
        isRotate = true;
    }

    void Update()
    {
        if (zoomingIn)
        {
            orthographicCamera.orthographicSize -= zoomSensitivity * Time.deltaTime;
            orthographicCamera.orthographicSize = Mathf.Clamp(orthographicCamera.orthographicSize, 0.1f, 100f);
        }
        else if (zoomingOut)
        {
            orthographicCamera.orthographicSize += zoomSensitivity * Time.deltaTime;
            orthographicCamera.orthographicSize = Mathf.Clamp(orthographicCamera.orthographicSize, 0.1f, 100f);
        }

        if (isRotate)
        {
            parentObject.Rotate(0, speed * Time.deltaTime, 0, Space.World);
            // Debug.Log(videoPinTableGenerator.rows + " " +
            // videoPinTableGenerator.columns);
            orthographicCamera.orthographicSize = Math.Max(videoPinTableGenerator.rows, videoPinTableGenerator.columns) <= 11 ? 7: Math.Max(videoPinTableGenerator.rows / 2, videoPinTableGenerator.columns / 2);
            // parentObject.position = 3 * Vector3.up;
        }
    }
}

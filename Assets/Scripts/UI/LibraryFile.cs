using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryFile : MonoBehaviour
{
    private string path;

    public TMP_Text text;
    public Button button;


    // public USPinTable.PinTableManager pinTableManager;
    // MonoBehaviour scripts don't use constructors in Unity
    // Use Start or Awake for initialization
    void Start()
    {
        // Extract file name without extension from path and set it to the text field
        if (text != null)
        {
            text.text = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        // Set up the button click event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        Debug.Log("Full path: " + path);
        if (USPinTable.PinTableManager.Instance != null)
        {
            USPinTable.PinTableManager.Instance.LoadDataFromFile(path);
        }

        // pinTableManager.LoadDataFromFile(path);
    }

    // Method to set the path from outside this class
    public void SetPath(string newPath)
    {
        path = newPath;

        // Update the text if needed
        if (text != null)
        {
            text.text = System.IO.Path.GetFileNameWithoutExtension(newPath);
        }
    }
}

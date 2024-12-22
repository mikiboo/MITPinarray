using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LibraryFileWithoutPlayButton : MonoBehaviour
{

    [HideInInspector] public string path;
    public TMP_Text text;
    public Button button;
    public Outline buttonOutline;

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

            // Get the Outline component or add it if it doesn't exist

        }
    }

    void OnButtonClick()
    {
        Debug.Log("Full path: " + path);
        LibraryFileWithoutPlayButton tempLibraryFile = AnimationManager.Instance.SelectedLibraryFile;
        if (tempLibraryFile != null)
        {
            tempLibraryFile.buttonOutline.enabled = false;
        }
        AnimationManager.Instance.SelectedLibraryFile = this;
        buttonOutline.enabled = true;

        // Toggle the outline
        // if (buttonOutline != null)
        // {
        //     buttonOutline.enabled = !buttonOutline.enabled;
        // }
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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> savedFilePaths;
    public GameObject libraryFileWithoutPlayButtonPrefab;
    public GameObject libraryAnimationBlockPrefab;
    public GameObject TimeBlockPrefab;
    public RectTransform libraryFileWithoutPlayButtonPrefabParent;
    [HideInInspector] public LibraryFileWithoutPlayButton SelectedLibraryFile;
    public RectTransform UICmanagerParent;
    public static AnimationManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [HideInInspector] private string pathsFilePath;
    void Start()
    {
        savedFilePaths = new List<string>();
        string saveDirectory = Application.dataPath + "/saved";

        pathsFilePath = Path.Combine(saveDirectory, "savedFilePaths.txt");
        Debug.Log(saveDirectory);
        Debug.Log(Directory.Exists(saveDirectory));
        try
        {
            if (!Directory.Exists(saveDirectory))
            {
                Debug.Log("Done created");
                Directory.CreateDirectory(saveDirectory);
            }
        }
        catch (IOException ex)
        {

            Debug.Log(ex.Message);
        }
        if (File.Exists(pathsFilePath))
        {
            string[] paths = File.ReadAllLines(pathsFilePath);
            foreach (var path in paths)
            {
                AddSavedFilePath(path);
                GameObject newLibraryFile = Instantiate(libraryFileWithoutPlayButtonPrefab, libraryFileWithoutPlayButtonPrefabParent);
                if (newLibraryFile.TryGetComponent<LibraryFileWithoutPlayButton>(out var libraryFileComponent))
                {
                    libraryFileComponent.SetPath(path);
                }
            }
            RemoveNonExistingPaths();
        }

    }
    private void RemoveNonExistingPaths()
    {
        List<string> existingPaths = new List<string>();
        foreach (var path in savedFilePaths)
        {
            if (File.Exists(path))
            {
                existingPaths.Add(path);
            }
            else
            {
                Debug.Log("Removing non-existing path: " + path);
            }
        }
        savedFilePaths = existingPaths;
        File.WriteAllLines(pathsFilePath, savedFilePaths);
    }
    private void AddSavedFilePath(string path)
    {
        // savedFilePaths.Add(path);// Add the path to the list
        // Debug.Log(pathsFilePath);
        // File.WriteAllLines(pathsFilePath, savedFilePaths);
        if (File.Exists(path))
        {
            savedFilePaths.Add(path); // Add the path to the list only if it exists
            Debug.Log("Path exists and added: " + path);
            File.WriteAllLines(pathsFilePath, savedFilePaths);
        }
        else
        {
            Debug.Log("Path does not exist: " + path);
        }
    }
    public void AddShapeBlock()
    {
        // savedFilePaths.Add(path);// Add the path to the list
        // Debug.Log(pathsFilePath);
        // File.WriteAllLines(pathsFilePath, savedFilePaths);
        // if (File.Exists(path))
        // {
        //     savedFilePaths.Add(path); // Add the path to the list only if it exists
        //     Debug.Log("Path exists and added: " + path);
        //     File.WriteAllLines(pathsFilePath, savedFilePaths);
        // }
        // else
        // {
        //     Debug.Log("Path does not exist: " + path);
        // }
        // Instantiate the new library file
        if (SelectedLibraryFile == null)
            return;
        GameObject newLibraryFile = Instantiate(libraryAnimationBlockPrefab, UICmanagerParent);

        // Set the position
        RectTransform rectTransform = newLibraryFile.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(148, 200);

        // If the component exists, set the path
        if (newLibraryFile.TryGetComponent<LibraryFileWithoutPlayButton>(out var libraryFileComponent))
        {
            Debug.Log($"SelectedLibraryFile.text.text: {SelectedLibraryFile.path}");
            libraryFileComponent.SetPath(SelectedLibraryFile.path);
        }

    }
    public void AddTimeBlock()
    {
        // savedFilePaths.Add(path);// Add the path to the list
        // Debug.Log(pathsFilePath);
        // File.WriteAllLines(pathsFilePath, savedFilePaths);
        // if (File.Exists(path))
        // {
        //     savedFilePaths.Add(path); // Add the path to the list only if it exists
        //     Debug.Log("Path exists and added: " + path);
        //     File.WriteAllLines(pathsFilePath, savedFilePaths);
        // }
        // else
        // {
        //     Debug.Log("Path does not exist: " + path);
        // }
        // Instantiate the new library file
        GameObject newLibraryFile = Instantiate(TimeBlockPrefab, UICmanagerParent);

        // Set the position
        RectTransform rectTransform = newLibraryFile.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(148, 200);

        // // If the component exists, set the path
        // if (newLibraryFile.TryGetComponent<LibraryFileWithoutPlayButton>(out var libraryFileComponent))
        // {
        //     libraryFileComponent.SetPath("path");
        // }


    }
    // Update is called once per frame
    void Update()
    {

    }
}

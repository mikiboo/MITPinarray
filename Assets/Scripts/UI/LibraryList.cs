using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryList : MonoBehaviour
{
    // Start is called before the first frame update
    public USPinTable.PinTableManager pinTableManager;
    public GameObject libraryFilePrefab;
    public void Execute()
    {
        foreach (var path in pinTableManager.savedFilePaths)
        {
            GameObject newLibraryFile = Instantiate(libraryFilePrefab, this.transform);
            if (newLibraryFile.TryGetComponent<LibraryFile>(out var libraryFileComponent))
            {
                libraryFileComponent.SetPath(path);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

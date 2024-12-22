using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.SimpleFileBrowserForWindows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using USPinTable;

namespace USPinTable
{
    public class FileManager : MonoBehaviour
    {
        public PinTableManager pinTableManager;

        public void OpenFile()
        {
            StartCoroutine(WindowsFileBrowser.OpenFile("Open", null, "JSON file", new[] { ".json" },
                (success, path, bytes) =>
                {
                    if (success)
                    {
                        pinTableManager.LoadDataFromFile(path);
                    }
                    else
                    {
                        Debug.LogError("Cannot access " + path);
                    }
                }));
        }

        public void SaveFile()
        {
            
            StartCoroutine(WindowsFileBrowser.SaveFile("Save", null, "Untitled", "JSON file", ".json",
                Encoding.UTF8.GetBytes("Hello!"),
                (success, path) =>
                {
                    if (success)
                    {
                        pinTableManager.SaveDataToFile(path);
                    }
                    else
                    {
                        Debug.Log("Cannot access " + path);
                    }
                }));
        }
    }
}

using System.Text;
using UnityEngine;

namespace Assets.SimpleFileBrowserForWindows
{
    public class Example : MonoBehaviour
    {
        public void OpenFile()
        {
            StartCoroutine(WindowsFileBrowser.OpenFile("Open", null, "Text file", new[] { ".txt" },
                (success, path, bytes) =>
                {
                    Debug.Log(success);
                    Debug.Log(path);
                    Debug.Log(bytes?.Length);
                }));
        }

        public void SaveFile()
        {
            StartCoroutine(WindowsFileBrowser.SaveFile("Save", null, "Untitled", "Text file", ".txt", Encoding.UTF8.GetBytes("Hello!"),
                (success, path) =>
                {
                    Debug.Log(success);
                    Debug.Log(path);
                }));
        }
    }
}
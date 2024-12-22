using UnityEngine;
using UnityEngine.UI;

public class ButtonTabView : MonoBehaviour
{
    // Assign these from the Inspector
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button1Deactive;
    public GameObject button2Deactive;
    public GameObject button3Deactive;
    public GameObject gameObject1;
    public GameObject gameObject2;
    public GameObject gameObject3;

    void Start()
    {
        // Add click listeners to the buttons inside GameObjects
        button1.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(1));
        button2.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(2));
        button3.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(3));
        button1Deactive.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(1));
        button2Deactive.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(2));
        button3Deactive.GetComponent<Button>().onClick.AddListener(() => ToggleTabs(3));

        // Optionally, start with the first tab active
        ToggleTabs(1);
    }

    void ToggleTabs(int index)
    {
        // Deactivate all GameObjects
        gameObject1.SetActive(false);
        gameObject2.SetActive(false);
        gameObject3.SetActive(false);

        // Reset all buttons to inactive state
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        button1Deactive.SetActive(true);
        button2Deactive.SetActive(true);
        button3Deactive.SetActive(true);
        Debug.Log($"index clicked {index}");

        // Activate the selected GameObject and its active button based on the index
        switch (index)
        {
            case 1:
                gameObject1.SetActive(true);
                button1.SetActive(true);
                button1Deactive.SetActive(false);
                break;
            case 2:
                gameObject2.SetActive(true);
                button2.SetActive(true);
                button2Deactive.SetActive(false);
                break;
            case 3:
                gameObject3.SetActive(true);
                button3.SetActive(true);
                button3Deactive.SetActive(false);
                break;
        }
    }
}

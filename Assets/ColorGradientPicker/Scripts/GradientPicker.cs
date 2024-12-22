using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USPinTable;

public class GradientPicker : MonoBehaviour
{
    /// <summary>
    /// Event that gets called by the GradientPicker.
    /// </summary>
    /// <param name="g">received Gradient</param>
    public delegate void GradientEvent(Gradient g);

    private static GradientPicker instance;
    /// <summary>
    /// True when the GradientPicker is closed
    /// </summary>
    public static bool done = true;
    private List<GameObject> createdGradientPanels = new List<GameObject>();
    public PinTableGenerator pinTableGenerator;
    //onGradientChanged Event
    private static GradientEvent onGC;
    //onGradientSelected Event
    private static GradientEvent onGS;

    //Gradient before editing
    private static Gradient originalGradient;
    //current Gradient
    private static Gradient modifiedGradient;

    //key template
    private GameObject key;

    private static bool interact;


    //all these objects only work on Prefab
    private InputField positionComponent;
    private Image colorComponent;
    private Transform alphaComponent;

    private List<Slider> colorKeyObjects;
    private List<GradientColorKey> colorKeys;
    private int selectedColorKey;
    private List<Slider> alphaKeyObjects;
    private List<GradientAlphaKey> alphaKeys;
    private int selectedAlphaKey;
    public GameObject gradientPanelPrefab;
    public Transform gradientPanelParent;
    private int numberOfCreatedGradientPanel = 4;
    private void Awake()
    {
        instance = this;
        key = transform.GetChild(2).gameObject;
        positionComponent = transform.parent.GetChild(3).GetComponent<InputField>();
        colorComponent = transform.parent.GetChild(4).GetComponent<Image>();
        alphaComponent = transform.parent.GetChild(5);
        transform.parent.gameObject.SetActive(false);
    }
    //  void Start()
    // {
    //     // Create a new GradientPicker instance
    //     GradientPicker.Create(new Gradient(), "Select two colors for the gradient", null, null);
    // }
    /// <summary>
    /// Creates a new GradiantPicker
    /// </summary>
    /// <param name="original">Color before editing</param>
    /// <param name="message">Display message</param>
    /// <param name="onGradientChanged">Event that gets called when the gradient gets modified</param>
    /// <param name="onGradientSelected">Event that gets called when one of the buttons done or cancel gets pressed</param>
    /// <returns>False if the instance is already running</returns>
    public static bool Create(Gradient original, string message, GradientEvent onGradientChanged, GradientEvent onGradientSelected)
    {
        if (instance is null)
        {
            Debug.LogError("No Gradientpicker prefab active on 'Start' in scene");
            return false;
        }
        if (done)
        {
            done = false;
            originalGradient = new Gradient();
            originalGradient.SetKeys(original.colorKeys, original.alphaKeys);
            modifiedGradient = new Gradient();
            modifiedGradient.SetKeys(original.colorKeys, original.alphaKeys);
            onGC = onGradientChanged;
            onGS = onGradientSelected;
            instance.transform.parent.gameObject.SetActive(true);
            instance.transform.parent.GetChild(0).GetChild(0).GetComponent<Text>().text = message;
            instance.Setup();
            return true;
        }
        else
        {
            Done();
            return false;
        }
    }
    //Setup new GradientPicker
    private void Setup()
    {
        interact = false;
        colorKeyObjects = new List<Slider>();
        colorKeys = new List<GradientColorKey>();
        alphaKeyObjects = new List<Slider>();
        alphaKeys = new List<GradientAlphaKey>();
        foreach (GradientColorKey k in originalGradient.colorKeys)
        {
            CreateColorKey(k);
        }
        // foreach (GradientAlphaKey k in originalGradient.alphaKeys)
        // {
        //     CreateAlphaKey(k);
        // }
        CalculateTexture();
        interact = true;
    }
    //creates a ColorKey UI object
    private void CreateColorKey(GradientColorKey k)
    {
        Debug.Log("colorKeys.Count " + k.color);
        if (colorKeys.Count < 2)
        {
            Slider s = Instantiate(key, transform.position, new Quaternion(), transform).GetComponent<Slider>();

            // Calculate the x position based on the time value (assuming a horizontal layout)
            float xPosition = Mathf.Lerp(-290f, 290f, k.time); // Adjust the range as needed

            // Set the anchored position based on the calculated x position
            ((RectTransform)s.transform).anchoredPosition = new Vector2(xPosition, -63f);

            s.name = "ColorKey";
            s.gameObject.SetActive(true);
            s.value = k.time;

            s.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = k.color;

            // Locate the handle and adjust its size
            RectTransform handleRectTransform = s.handleRect;
            handleRectTransform.sizeDelta = new Vector2(30f, 30f);
            s.onValueChanged.AddListener((value) => s.value = k.time);
            colorKeyObjects.Add(s);
            colorKeys.Add(k);
            ChangeSelectedColorKey(colorKeys.Count - 1);
        }
    }
    //checks if new ColorKey should be created
    public void CreateNewColorKey(float time)
    {
        if (Input.GetMouseButtonDown(0))
        {
            interact = false;
            CreateColorKey(new GradientColorKey(modifiedGradient.Evaluate(time), time));
            interact = true;
        }
    }
    //creates a AlphaKey UI object
    private void CreateAlphaKey(GradientAlphaKey k)
    {
        if (alphaKeys.Count < 8)
        {
            Slider s = Instantiate(key, transform.position, new Quaternion(), transform).GetComponent<Slider>();
            ((RectTransform)s.transform).anchoredPosition = new Vector2(0, 25f);
            s.transform.GetChild(0).GetChild(0).rotation = new Quaternion();
            s.name = "AlphaKey";
            s.gameObject.SetActive(true);
            s.value = k.time;
            s.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(k.alpha, k.alpha, k.alpha, 1f);
            alphaKeyObjects.Add(s);
            alphaKeys.Add(k);
            ChangeSelectedAlphaKey(alphaKeys.Count - 1);
        }
    }
    //checks if new AlphaKey should be created
    public void CreateNewAlphaKey(float time)
    {
        if (Input.GetMouseButtonDown(0))
        {
            interact = false;
            CreateAlphaKey(new GradientAlphaKey(modifiedGradient.Evaluate(time).a, time));
            interact = true;
        }
    }

    private void CalculateTexture()
    {
        Color[] g = new Color[325];
        for (int i = 0; i < g.Length; i++)
        {
            g[i] = modifiedGradient.Evaluate(i / (float)g.Length);
        }
        Texture2D tex = new Texture2D(g.Length, 1)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        tex.SetPixels(g);
        tex.Apply();
        GetComponent<RawImage>().texture = tex;
        onGC?.Invoke(modifiedGradient);
    }
    //accessed by alpha Slider
    public void SetAlpha(float value)
    {
        if (interact)
        {
            alphaKeys[selectedAlphaKey] = new GradientAlphaKey(value, alphaKeys[selectedAlphaKey].time);
            modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            CalculateTexture();
            alphaComponent.GetChild(4).GetComponent<InputField>().text = Mathf.RoundToInt(value * 255f).ToString();
            alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(value, value, value, 1f);
        }
    }
    //accessed by alpha InputField
    public void SetAlpha(string value)
    {
        alphaComponent.GetComponent<Slider>().value = Mathf.Clamp(int.Parse(value), 0, 255) / 255f;
        CalculateTexture();
    }

    private void ChangeSelectedColorKey(int value)
    {
        // value = 1;
        Debug.Log($"ChangeSelectedColorKey {selectedColorKey} {value}");
        if (colorKeyObjects.Count() > selectedColorKey)
        {
            colorKeyObjects[selectedColorKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        if (alphaKeyObjects.Count() > 0)
        {
            alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        colorKeyObjects[value].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
        if (selectedColorKey != value && !ColorPicker.done)
        {
            ColorPicker.Done();
        }
        selectedColorKey = value;
        colorKeyObjects[value].Select();
    }

    private void ChangeSelectedAlphaKey(int value)
    {
        // if (alphaKeyObjects.Count > selectedAlphaKey)
        // {
        //     alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        // }
        // if (colorKeyObjects.Count > 0)
        // {
        //     colorKeyObjects[selectedColorKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        // }
        // alphaKeyObjects[value].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
        Debug.Log($"ChangeSelectedAlphaKey {selectedAlphaKey} {value}");
        selectedAlphaKey = value;
        alphaKeyObjects[value].Select();
    }
    //checks if Key can be deleted
    public void CheckDeleteKey(Slider s)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (s.name == "ColorKey" && colorKeys.Count > 2)
            {
                if (!ColorPicker.done)
                {
                    ColorPicker.Done();
                    return;
                }
                int index = colorKeyObjects.IndexOf(s);
                Destroy(colorKeyObjects[index].gameObject);
                colorKeyObjects.RemoveAt(index);
                colorKeys.RemoveAt(index);
                if (index <= selectedColorKey)
                {
                    ChangeSelectedColorKey(selectedColorKey - 1);
                }
                modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
                CalculateTexture();
            }
            if (s.name == "AlphaKey" && alphaKeys.Count > 2)
            {
                int index = alphaKeyObjects.IndexOf(s);
                Destroy(alphaKeyObjects[index].gameObject);
                alphaKeyObjects.RemoveAt(index);
                alphaKeys.RemoveAt(index);
                if (index <= selectedAlphaKey)
                {
                    ChangeSelectedAlphaKey(selectedAlphaKey - 1);
                }
                modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
                CalculateTexture();
            }
        }
    }
    //changes Selected Key
    public void Select()
    {
        Slider s = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        s.transform.SetAsLastSibling();
        Debug.Log($"Select {s.name}");
        if (s.name == "ColorKey")
        {
            ChangeSelectedColorKey(colorKeyObjects.IndexOf(s));
            alphaComponent.gameObject.SetActive(false);
            colorComponent.gameObject.SetActive(true);
            positionComponent.text = Mathf.RoundToInt(colorKeys[selectedColorKey].time * 100f).ToString();
            colorComponent.GetComponent<Image>().color = colorKeys[selectedColorKey].color;
        }
        // else
        // {
        //     ChangeSelectedAlphaKey(alphaKeyObjects.IndexOf(s));
        //     colorComponent.gameObject.SetActive(false);
        //     alphaComponent.gameObject.SetActive(true);
        //     positionComponent.text = Mathf.RoundToInt(alphaKeys[selectedAlphaKey].time * 100f).ToString();
        //     alphaComponent.GetComponent<Slider>().value = alphaKeys[selectedAlphaKey].alpha;
        //     alphaComponent.GetChild(4).GetComponent<InputField>().text = Mathf.RoundToInt(alphaKeys[selectedAlphaKey].alpha * 255f).ToString();
        // }
    }
    //accessed by position Slider
    public void SetTime(float time)
    {
        if (interact)
        {
            Slider s = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            if (s.name == "ColorKey")
            {
                int index = colorKeyObjects.IndexOf(s);
                colorKeys[index] = new GradientColorKey(colorKeys[index].color, time);
            }
            else
            {
                int index = alphaKeyObjects.IndexOf(s);
                alphaKeys[index] = new GradientAlphaKey(alphaKeys[index].alpha, time);
            }
            modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            CalculateTexture();
            positionComponent.text = Mathf.RoundToInt(time * 100f).ToString();
        }
    }
    //accessed by position InputField
    public void SetTime(string time)
    {
        interact = false;
        float t = Mathf.Clamp(int.Parse(time), 0, 100) * 0.01f;
        if (colorComponent.gameObject.activeSelf)
        {
            colorKeyObjects[selectedColorKey].value = t;
            colorKeys[selectedColorKey] = new GradientColorKey(colorKeys[selectedColorKey].color, t);
        }
        else
        {
            alphaKeyObjects[selectedAlphaKey].value = t;
            alphaKeys[selectedAlphaKey] = new GradientAlphaKey(alphaKeys[selectedAlphaKey].alpha, t);
        }
        modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        CalculateTexture();
        interact = true;
    }
    //choose color button call
    public void ChooseColor()
    {
        ColorPicker.Create(colorKeys[selectedColorKey].color, "Gradient Color Key", (c) => UpdateColor(selectedColorKey, c), null);
    }

    private void UpdateColor(int index, Color c)
    {
        interact = false;
        colorKeys[index] = new GradientColorKey(c, colorKeys[index].time);
        colorKeyObjects[index].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = c;
        colorComponent.color = c;
        modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        CalculateTexture();
        interact = true;
    }
    //cancel button call
    public void CCancel()
    {
        Cancel();
    }
    /// <summary>
    /// Manually cancel the GradientPicker and recovers the default value
    /// </summary>
    // public static void Cancel()
    // {
    //     //modify this
    // }
    //done button call
    public void CDone()
    {
        Done();
    }
    /// <summary>
    /// Manually close the GradientPicker and apply the selected color
    /// </summary>
    // public static void Done()
    // {
    //     Debug.Log(ColorPicker.done);

    //     // Ensure that the color keys are valid and there are exactly two keys
    //     if (instance.colorKeys.Count == 2)
    //     {
    //         // Retrieve the two colors from the colorKeys list
    //         Color firstColor = instance.colorKeys[0].color;
    //         Color secondColor = instance.colorKeys[1].color;

    //         // Output the two colors
    //         Debug.Log("First Color: " + firstColor);
    //         Debug.Log("Second Color: " + secondColor);

    //         // Optionally, you can invoke a method or event to handle these colors
    //         // For example, if you want to trigger an event:
    //         // onGS?.Invoke(modifiedGradient);  // Pass the gradient that includes these colors

    //         // If you want to store them somewhere, you can do that here as well
    //     }
    //     else
    //     {
    //         Debug.LogError("Gradient does not have exactly two color keys.");
    //     }
    //     // if (!ColorPicker.done)
    //     //     ColorPicker.Done();
    //     // foreach (Slider s in instance.colorKeyObjects)
    //     // {
    //     //     Destroy(s.gameObject);
    //     // }
    //     // foreach (Slider s in instance.alphaKeyObjects)
    //     // {
    //     //     Destroy(s.gameObject);
    //     // }
    //     // instance.colorKeyObjects = null;
    //     // instance.colorKeys = null;
    //     // instance.alphaKeyObjects = null;
    //     // instance.alphaKeys = null;
    //     // done = true;
    //     // onGC?.Invoke(modifiedGradient);
    //     // onGS?.Invoke(modifiedGradient);
    //     // instance.transform.parent.gameObject.SetActive(false);
    // }
    // public static void Done()
    // {
    //     Debug.Log(ColorPicker.done);

    //     // Ensure that the color keys are valid and there are exactly two keys
    //     if (instance.colorKeys.Count == 2)
    //     {
    //         // Retrieve the two colors from the colorKeys list
    //         Color firstColor = instance.colorKeys[0].color;
    //         Color secondColor = instance.colorKeys[1].color;

    //         // Output the two colors
    //         Debug.Log("First Color: " + firstColor);
    //         Debug.Log("Second Color: " + secondColor);

    //         // Generate a texture that represents the gradient
    //         Texture2D gradientTexture = GenerateGradientTexture(firstColor, secondColor);

    //         // Create a UI panel to display the gradient
    //         GameObject gradientPanel = new GameObject("GradientPanel");
    //         gradientPanel.transform.SetParent(instance.transform.parent, false); // Parent it to the same parent as the GradientPicker

    //         // Set the panel's size and position
    //         RectTransform rectTransform = gradientPanel.AddComponent<RectTransform>();
    //         rectTransform.sizeDelta = new Vector2(200f, 50f); // Adjust size as needed
    //         rectTransform.anchoredPosition = new Vector2(0, -100f); // Adjust position as needed

    //         // Add an Image component to the panel and apply the gradient texture
    //         Image gradientImage = gradientPanel.AddComponent<Image>();
    //         gradientImage.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, gradientTexture.width, gradientTexture.height), new Vector2(0.5f, 0.5f));
    //     }
    //     else
    //     {
    //         Debug.LogError("Gradient does not have exactly two color keys.");
    //     }

    //     // Clean up and finalize the gradient picker
    //     // (Additional cleanup code here, if needed)
    // }

    // private static Texture2D GenerateGradientTexture(Color firstColor, Color secondColor)
    // {
    //     int width = 256; // Width of the texture
    //     int height = 1;  // Height of the texture, could be more if you want a thicker gradient

    //     Texture2D texture = new Texture2D(width, height);

    //     // Create a gradient from firstColor to secondColor
    //     for (int i = 0; i < width; i++)
    //     {
    //         Color color = Color.Lerp(firstColor, secondColor, i / (float)width);
    //         for (int j = 0; j < height; j++)
    //         {
    //             texture.SetPixel(i, j, color);
    //         }
    //     }

    //     texture.Apply();
    //     return texture;
    // }
    public static void Done()
    {
        GradientPicker instance = FindObjectOfType<GradientPicker>();
        if (instance.numberOfCreatedGradientPanel < 1)
        {
            return;
        }
        instance.numberOfCreatedGradientPanel--;
        if (instance.colorKeys.Count == 2)
        {
            // Retrieve the two colors from the colorKeys list
            Color firstColor = instance.colorKeys[0].color;
            Color secondColor = instance.colorKeys[1].color;

            // Output the two colors
            Debug.Log("First Color: " + firstColor);
            Debug.Log("Second Color: " + secondColor);

            // Instantiate the prefab
            GameObject gradientPanel = Instantiate(instance.gradientPanelPrefab, instance.gradientPanelParent.transform);

            // Adjust the RectTransform of the instantiated prefab
            RectTransform rectTransform = gradientPanel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -100f);  // Adjust position if needed

            // Get the Image component and set the gradient texture
            Image gradientImage = gradientPanel.GetComponent<Image>();
            gradientImage.sprite = GenerateGradientSprite(firstColor, secondColor);


            Button button = gradientPanel.GetComponent<Button>();
            if (button == null)
            {
                button = gradientPanel.AddComponent<Button>();
            }

            // Attach a listener to the button to log the colors when clicked
            button.onClick.AddListener(() => LogColors(firstColor, secondColor));
            // Add the created gradient panel to the list
            instance.createdGradientPanels.Add(gradientPanel);
        }
        else
        {
            Debug.LogError("Gradient does not have exactly two color keys.");
        }
    }
    private static void LogColors(Color firstColor, Color secondColor)
    {
        Debug.Log($"Button clicked! First Color: {firstColor}, Second Color: {secondColor}");
        instance.pinTableGenerator.UpdateColorMaxMin(firstColor, secondColor);
    }
    private static Sprite GenerateGradientSprite(Color firstColor, Color secondColor)
    {
        int width = 256;
        int height = 50;  // Adjust height to match the prefab

        Texture2D texture = new Texture2D(width, height);

        // Create a gradient from firstColor to secondColor
        for (int i = 0; i < width; i++)
        {
            Color color = Color.Lerp(firstColor, secondColor, i / (float)width);
            for (int j = 0; j < height; j++)
            {
                texture.SetPixel(i, j, color);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static void Cancel()
    {
        GradientPicker instance = FindObjectOfType<GradientPicker>();

        // Check if there are any created panels to destroy
        if (instance.createdGradientPanels.Count > 0)
        {
            // Get the last created panel
            GameObject lastPanel = instance.createdGradientPanels[instance.createdGradientPanels.Count - 1];

            // Destroy the last created panel
            Destroy(lastPanel);

            // Remove it from the list
            instance.createdGradientPanels.RemoveAt(instance.createdGradientPanels.Count - 1);

            // Increase the number of allowed panels to create
            instance.numberOfCreatedGradientPanel++;
        }

        // Add any other cleanup logic here if necessary
    }


}

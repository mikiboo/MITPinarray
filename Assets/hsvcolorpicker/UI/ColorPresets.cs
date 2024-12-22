using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USPinTable;
namespace HSVPicker
{
    public class ColorPresets : MonoBehaviour
    {
        public ColorPicker picker;
        public GameObject[] presets;
        public Image createPresetImage;

        private ColorPresetList _colors;
        public PinTableGenerator pinTableGenerator;
        void Awake()
        {
            //		picker.onHSVChanged.AddListener(HSVChanged);
            picker.onValueChanged.AddListener(ColorChanged);
        }

        void Start()
        {
            if (picker == null)
            {
                Debug.LogError("ColorPresets: Picker is not assigned!");
            }

            GenerateDefaultPresetColours();

            if (_colors == null)
            {
                Debug.LogError("ColorPresets: Failed to initialize colors list.");
            }
        }


        void OnEnable()
        {
            // if the picker is set to regenerate its settings on open, then
            // regenerate the default picker options.
            if (picker.Setup.RegenerateOnOpen)
            {
                GenerateDefaultPresetColours();
            }
        }

        private void GenerateDefaultPresetColours()
        {
            List<Color> empty = new List<Color>();
            _colors = ColorPresetManager.Get(picker.Setup.PresetColorsId);

            if (_colors != null)
            {
                _colors.UpdateList(empty);
                Debug.Log($"_colors.Colors.Count {_colors.Colors.Count}");
                _colors.OnColorsUpdated += OnColorsUpdate;
                OnColorsUpdate(_colors.Colors);
            }
            else
            {
                Debug.LogError("Failed to initialize _colors");
            }
        }


        private void OnColorsUpdate(List<Color> colors)
        {
            for (int cnt = 0; cnt < presets.Length; cnt++)
            {

                if (colors.Count <= cnt)
                {

                    presets[cnt].SetActive(false);
                    continue;
                }


                presets[cnt].SetActive(true);
                presets[cnt].GetComponent<Image>().color = colors[cnt];

            }

            createPresetImage.gameObject.SetActive((colors.Count < presets.Length) && picker.Setup.UserCanAddPresets);

        }

        public void CreatePresetButton()
        {
            _colors.AddColor(picker.CurrentColor);

            //      for (var i = 0; i < presets.Length; i++)
            //{
            //	if (!presets[i].activeSelf)
            //	{
            //		presets[i].SetActive(true);
            //		presets[i].GetComponent<Image>().color = picker.CurrentColor;
            //		break;
            //	}
            //}
        }
        public void DeletePresetButton()
        {
            Debug.Log($"DeletePresetButton {picker.CurrentColor}");
            if (picker.CurrentColor == null)
                return;
            _colors.RemoveColor();

            //      for (var i = 0; i < presets.Length; i++)
            //{
            //	if (!presets[i].activeSelf)
            //	{
            //		presets[i].SetActive(true);
            //		presets[i].GetComponent<Image>().color = picker.CurrentColor;
            //		break;
            //	}
            //}
        }
        public void PresetSelect(Image sender)
        {
            picker.CurrentColor = sender.color;
            // string hexColor = ColorUtility.ToHtmlStringRGBA();

            // Print the hex color to the console
            Color imgColorWithZeroOpacity = new Color(sender.color.r, sender.color.g, sender.color.b, 1f);

            // Create a new color with full opacity (1.0f)
            Color imgColorWithFullOpacity = new Color(sender.color.r, sender.color.g, sender.color.b, 1f);

            // Convert the color to HEX (optional, just for debugging)
            string hexColor = ColorUtility.ToHtmlStringRGB(imgColorWithFullOpacity);
            Debug.Log($"Color of the image in HEX with full opacity: #{hexColor}");

            // Update the color range with the new colors
            pinTableGenerator.UpdateColorMaxMin(imgColorWithZeroOpacity, imgColorWithFullOpacity);
        }

        // Not working, it seems ConvertHsvToRgb() is broken. It doesn't work when fed
        // input h, s, v as shown below.
        //	private void HSVChanged(float h, float s, float v)
        //	{
        //		createPresetImage.color = HSVUtil.ConvertHsvToRgb(h, s, v, 1);
        //	}
        private void ColorChanged(Color color)
        {
            createPresetImage.color = color;
        }

        private void OnDestroy()
        {
            if (picker != null)
            {
                picker.onValueChanged.RemoveListener(ColorChanged);
            }

            if (_colors != null)
            {
                _colors.OnColorsUpdated -= OnColorsUpdate;
            }
        }

    }


}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HSVPicker
{
    public static class ColorPresetManager
    {
        private static Dictionary<string, ColorPresetList> _presets = new Dictionary<string, ColorPresetList>();

        public static ColorPresetList Get(string listId = "default")
        {
            ColorPresetList preset;
            if (!_presets.TryGetValue(listId, out preset))
            {
                preset = new ColorPresetList(listId);
                _presets.Add(listId, preset);
            }

            return preset;
        }


    }

    public class ColorPresetList
    {
        public string ListId { get; private set; }
        public List<Color> Colors { get; private set; }

        public event UnityAction<List<Color>> OnColorsUpdated;

        public ColorPresetList(string listId, List<Color> colors = null)
        {
            if (colors == null)
            {
                colors = new List<Color>();
            }
            Debug.Log($"ColorPresetList {listId} {colors.Count}");
            Colors = colors;
            ListId = listId;
        }

        public void AddColor(Color color)
        {
            Colors.Add(color);
            if (OnColorsUpdated != null)
            {
                OnColorsUpdated.Invoke(Colors);
            }
        }
        public void RemoveColor()
        {
            if (Colors.Count > 0)
            {
                // Remove the last color in the list
                Colors.RemoveAt(Colors.Count - 1);

                // If there is a subscriber to the event, invoke the OnColorsUpdated event
                if (OnColorsUpdated != null)
                {
                    OnColorsUpdated.Invoke(Colors);
                }
            }
            else
            {
                Debug.Log("No colors to remove.");
            }
        }

        public void UpdateList(IEnumerable<Color> colors)
        {
            Colors.Clear();
            Colors.AddRange(colors);

            if (OnColorsUpdated != null)
            {
                OnColorsUpdated.Invoke(Colors);
            }
        }


    }
}

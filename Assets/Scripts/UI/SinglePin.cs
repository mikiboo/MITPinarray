using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace USPinTable
{
    public class SinglePin : MonoBehaviour
    {
        public int height = 0; // 0~350

        #region 3D pin table

        public SingleCube cube;

        private float maxHeight = 350f;
        #endregion

        #region UI pin table

        public Image image;

        public Outline outline;

        public Color colorMax = new(255f / 255f, 106f / 255f, 0f / 255f);
        public Color colorMin = new(255f / 255f, 217f / 255f, 190f / 255f);
        private int maxHeightInt;

        public TMP_Text text;
        private void Start()
        {
            maxHeightInt = (int)maxHeight;
        }
        public void Select()
        {
            outline.enabled = true;
        }

        public void Unselect()
        {
            outline.enabled = false;
        }

        public void OnClickPin()
        {
            outline.enabled = !outline.enabled;
        }
        public void UpdateColorRange(Color newColorMin, Color newColorMax)
        {
            colorMin = newColorMin;
            colorMax = newColorMax;
            UpdatePinColor();  // Update the color immediately based on the current height

            if (cube != null)
            {

                cube.UpdateColorRange(newColorMin, newColorMax); // Update the color range for the cube as well
            }
        }

        private void UpdatePinColor()
        {
            float fraction = height / maxHeight;
            Color interpolatedColor = Color.Lerp(colorMin, colorMax, fraction);
            if (image != null)
            {
                image.color = interpolatedColor;
            }
        }
        public void UpdateHeight(int height)
        {
            // Debug.Log($"update ${height}");
            this.height = Mathf.Clamp(height, 0, maxHeightInt);
            text.text = this.height.ToString();
            float fraction = this.height / maxHeight;
            Color interpolatedColor = Color.Lerp(colorMin, colorMax, fraction);
            // Debug.Log($"image: {interpolatedColor}");
            if (image != null)
            {
                image.color = interpolatedColor;
            }
            Debug.Log($"cube: {cube}");
            if (cube != null)
            {
                cube.UpdateHeight(height);
                cube.UpdateCubeColor(height);
            }
        }

        #endregion
    }
}

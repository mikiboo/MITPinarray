using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace USPinTable
{
    public class SingleModule : MonoBehaviour
    {
        public float height = 0; // 0~100
        public TMP_Text textField;
        #region 3D pin table


        public float maxHeight = 100f;
        #endregion

        #region UI pin table

        public Image image;

        public TextMeshPro inputText;
        public Color colorMax;
        public Color colorMin;
    
        // public void Select()
        // {
        //     outline.enabled = true;
        // }

        // public void Unselect()
        // {
        //     outline.enabled = false;
        // }

        // public void OnClickPin()
        // {
        //     outline.enabled = !outline.enabled;
        // }

        public void UpdateHeight(float height)
        {
            this.height = Mathf.Clamp(height, 0, maxHeight);
            float fraction = this.height / maxHeight;
            Color interpolatedColor = Color.Lerp(colorMin, colorMax, fraction);
            if (image != null)
            {
                image.color = interpolatedColor;
            }
            textField.text = this.height.ToString()+"°c";
        }

        #endregion
    }
}

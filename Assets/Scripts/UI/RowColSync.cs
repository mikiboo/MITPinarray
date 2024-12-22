using System.Collections;
using System.Collections.Generic;
using Ookii.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using USPinTable;

namespace USPinTable
{
    public class RowColSync : MonoBehaviour
    {
        public bool syncManager = true;

        // Reference to the PinTableManager
        // References to InputField GameObjects
        public TMP_InputField baseRowInput;
        public TMP_InputField baseColInput;
        public TMP_InputField rowInput;
        public TMP_InputField colInput;

        void Start()
        {
            if (syncManager)
            {
                SyncFromManager();
            }

            // Setup the listeners for each InputField
            if (baseRowInput != null && baseColInput != null)
            {
                baseRowInput.onValueChanged.AddListener(delegate { UpdateBaseRow(); });
                baseColInput.onValueChanged.AddListener(delegate { UpdateBaseCol(); });
            }

            if (rowInput != null && colInput != null)
            {
                rowInput.onValueChanged.AddListener(delegate { UpdateRow(); });
                colInput.onValueChanged.AddListener(delegate { UpdateCol(); });
            }
        }

        void SyncFromManager()
        {
            // Set the InputField text to match the current values in PinTableManager
            if (baseRowInput != null && baseColInput != null)
            {
                baseRowInput.text = GlobalManager.baseRow.ToString();
                baseColInput.text = GlobalManager.baseCol.ToString();
            }

            if (rowInput != null && colInput != null)
            {
                rowInput.text = GlobalManager.row.ToString();
                colInput.text = GlobalManager.col.ToString();
            }
        }

        void UpdateBaseRow()
        {
            if (int.TryParse(baseRowInput.text, out int newValue))
            {
                GlobalManager.baseRow = newValue;
            }
        }

        void UpdateBaseCol()
        {
            if (int.TryParse(baseColInput.text, out int newValue))
            {
                GlobalManager.baseCol = newValue;
            }
        }

        void UpdateRow()
        {
            if (int.TryParse(rowInput.text, out int newValue))
            {
                GlobalManager.row = newValue;
            }
        }

        void UpdateCol()
        {
            if (int.TryParse(colInput.text, out int newValue))
            {
                GlobalManager.col = newValue;
            }
        }
    }
}

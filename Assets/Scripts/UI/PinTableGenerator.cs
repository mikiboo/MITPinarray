using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Windows.Input;

namespace USPinTable
{
    public class PinTableGenerator : MonoBehaviour
    {
        public bool enable3D = true;

        public Vector2 pinPos;
        public RectTransform pinParent;
        public GameObject pinPrefab;
        public Prompt resetBoxDialog;
        public Button confirmResetButton;
        public int pinWidth = 50; // This is the width for each pin

        public Vector3 cubePos;
        public Transform cubeParent;
        public GameObject cubePrefab;
        public int cubeWidth = 50;
        public int cubeScale = 20;

        public int columns;
        public int rows;

        public SinglePin[,] PinTable;
        private SingleCube[,] CubeTable;
        public bool[,] ClickedPinTable;

        private void Start()
        {
            if (confirmResetButton != null)
            {
                confirmResetButton.onClick.AddListener(() => ResetHeight());
            }


        }
        private void OnDestroy()
        {
            if (confirmResetButton != null)
            {
                confirmResetButton.onClick.RemoveAllListeners();
            }
        }
        public void Generate(int x, int y)
        {
            if (enable3D)
            {
                ClearCube();
            }

            ClearGrid();
            if (enable3D)
            {
                GenerateCube(x, y);
            }

            GenerateGrid(x, y);
        }

        private void GenerateCube(int x, int y)
        {
            rows = x;
            columns = y;

            CubeTable = new SingleCube[rows, columns];

            // Calculate the initial position to start generating cubes from the bottom left corner
            Vector3 startOffset = new Vector3((cubeWidth * (columns - 1)) / 2.0f, 0, (cubeWidth * (rows - 1)) / 2.0f);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Calculate position for each cube to ensure the table is centered around the parent
                    Vector3 position = new Vector3(j * cubeWidth - startOffset.x, 0, -i * cubeWidth - startOffset.z);
                    var newCube = Instantiate(cubePrefab, position, Quaternion.identity, cubeParent).transform;
                    // hard code to have a gap between cubes
                    newCube.localScale = new Vector3(cubeWidth * 0.99f, cubeWidth * 0.99f, cubeWidth * 0.99f);
                    CubeTable[i, j] = newCube.GetComponent<SingleCube>();
                    CubeTable[i, j].scale = cubeScale;

                }
            }

            cubeParent.position = cubePos;
        }

        private void GenerateGrid(int x, int y)
        {
            rows = x;
            columns = y;

            PinTable = new SinglePin[rows, columns];
            ClickedPinTable = new bool[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Instantiate the pin prefab
                    GameObject newPin = Instantiate(pinPrefab, pinParent);
                    PinTable[i, j] = newPin.GetComponent<SinglePin>();

                    if (enable3D)
                    {
                        PinTable[i, j].cube = CubeTable[i, j];
                    }

                    // Add listener
                    var j1 = j;
                    var i1 = i;
                    newPin.GetComponent<Button>().onClick.AddListener(() => OnPinClicked(i1, j1));

                    // Set the position
                    RectTransform rectTransform = newPin.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(j * pinWidth, -i * pinWidth);

                    // Set the size
                    rectTransform.sizeDelta = new Vector2(pinWidth, pinWidth);

                }
            }
        }

        private void ClearCube()
        {
            if (CubeTable != null)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (CubeTable[i, j] != null)
                        {
                            // Destroy the GameObject the SinglePin component is attached to
                            Destroy(CubeTable[i, j].gameObject);
                        }
                    }
                }
            }

            // Optionally, clear the array itself
            CubeTable = null;

            cubeParent.position = Vector3.zero;
            cubeParent.rotation = Quaternion.identity;
        }

        private void ClearGrid()
        {
            if (PinTable != null)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (PinTable[i, j] != null)
                        {
                            // Destroy the GameObject the SinglePin component is attached to
                            Destroy(PinTable[i, j].gameObject);
                        }
                    }
                }
            }

            // Optionally, clear the array itself
            PinTable = null;
            pinParent.anchoredPosition = pinPos;
            pinParent.localScale = new Vector3(1, 1, 1);
        }

        #region Update related
        public void ShowConfirmResetButton()
        {
            resetBoxDialog.ShowPrompt();

        }
        public void ResetHeight()
        {

            Generate(rows, columns);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    ClickedPinTable[i, j] = false;
                    PinTable[i, j].Unselect();
                    PinTable[i, j].UpdateHeight(0);
                }
            }

        }

        public void UpdateHeight(string value)
        {
            if (!int.TryParse(value, out var height))
            {
                return;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (ClickedPinTable[i, j])
                    {
                        PinTable[i, j].UpdateHeight(height);
                    }
                }
            }
        }
        public void UpdateColorMaxMin(Color newColorMin, Color newColorMax)
        {


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (ClickedPinTable[i, j])
                    {
                        PinTable[i, j].UpdateColorRange(newColorMin, newColorMax);
                    }
                }
            }
        }

        public void OnPinClicked(int row, int col)
        {
            ClickedPinTable[row, col] = !ClickedPinTable[row, col];
            PinTable[row, col].OnClickPin();
        }

        public void SelectAll()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    PinTable[i, j].Select();
                    ClickedPinTable[i, j] = true;
                }
            }
        }

        public void UnselectAll()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    PinTable[i, j].Unselect();
                    ClickedPinTable[i, j] = false;
                }
            }
        }

        #endregion


        #region Height Getter & Setter

        public int[,] GetHeights()
        {
            int[,] heights = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (PinTable[i, j] != null)
                    {
                        heights[i, j] = PinTable[i, j].height;
                    }
                    else
                    {
                        // If the PinTable[i, j] is null, we assume the height is 0
                        heights[i, j] = 0;
                    }
                }
            }

            return heights;
        }

        public void SetHeights(int[,] heights)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (PinTable[i, j] != null)
                    {
                        PinTable[i, j].UpdateHeight(heights[i, j]);
                    }
                }
            }
        }

        public void SetColors(Color[,] colorMax, Color[,] colorMin)
        {
            Debug.Log(colorMax);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (PinTable[i, j] != null)
                    {
                        PinTable[i, j].UpdateColorRange(colorMin[i, j], colorMax[i, j]);
                    }
                }
            }
        }


        #endregion
    }
}

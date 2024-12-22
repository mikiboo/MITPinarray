using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Windows.Input;

namespace USPinTable
{
    public class PinTableTemperatureGenerator : MonoBehaviour
    {
        // public bool enable3D = true;

        public Vector2 pinPos;
        public RectTransform pinParent;
        public GameObject modulePrefab;
        public Prompt resetBoxDialog;
        public Button confirmResetButton;

        // public Vector3 cubePos;
        // public Transform cubeParent;
        // public GameObject cubePrefab;
        // public int cubeWidth = 50;
        // public int cubeScale = 20;

        private int columns;
        private int rows;

        public SingleModule[,] PinTable;
        private void Awake()
        {
            confirmResetButton.onClick.AddListener(() => ResetHeight());
        }
        public void Generate(int x, int y)
        {
            // if (enable3D)
            // {
            //     ClearCube();
            // }

            ClearGrid();
            // if (enable3D)
            // {
            //     GenerateCube(x, y);
            // }

            GenerateGrid(x, y);
        }

        // private void GenerateCube(int x, int y)
        // {
        //     rows = x;
        //     columns = y;

        //     CubeTable = new SingleCube[rows, columns];

        //     // Calculate the initial position to start generating cubes from the bottom left corner
        //     Vector3 startOffset = new Vector3((cubeWidth * (columns - 1)) / 2.0f, 0, (cubeWidth * (rows - 1)) / 2.0f);

        //     for (int i = 0; i < rows; i++)
        //     {
        //         for (int j = 0; j < columns; j++)
        //         {
        //             // Calculate position for each cube to ensure the table is centered around the parent
        //             Vector3 position = new Vector3(j * cubeWidth - startOffset.x, 0, -i * cubeWidth - startOffset.z);
        //             var newCube = Instantiate(cubePrefab, position, Quaternion.identity, cubeParent).transform;
        //             // hard code to have a gap between cubes
        //             newCube.localScale = new Vector3(cubeWidth * 0.99f, cubeWidth * 0.99f, cubeWidth * 0.99f);
        //             CubeTable[i, j] = newCube.GetComponent<SingleCube>();
        //             CubeTable[i, j].scale = cubeScale;
        //         }
        //     }

        //     cubeParent.position = cubePos;
        // }

        private int horizontalSpacing = 20; // Define horizontal spacing
        private int verticalSpacing = 20; // Define vertical spacing

        private void GenerateGrid(int x, int y)
        {
            rows = x;
            columns = y;

            PinTable = new SingleModule[rows, columns];

            // Get the original size of the modulePrefab
            RectTransform prefabRectTransform = modulePrefab.GetComponent<RectTransform>();
            float originalWidth = prefabRectTransform.sizeDelta.x;
            float originalHeight = prefabRectTransform.sizeDelta.y;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Instantiate the module prefab
                    GameObject newModule = Instantiate(modulePrefab, pinParent);
                    PinTable[i, j] = newModule.GetComponent<SingleModule>();

                    // Add listener
                    var j1 = j;
                    var i1 = i;

                    // Set the position
                    RectTransform rectTransform = newModule.GetComponent<RectTransform>();
                    float xPosition = j * (originalWidth + horizontalSpacing);
                    float yPosition = -i * (originalHeight + verticalSpacing);
                    rectTransform.anchoredPosition = new Vector2(xPosition, yPosition);

                    // Set the size (if needed, you can skip this if you want to keep the original size)
                    rectTransform.sizeDelta = new Vector2(originalWidth, originalHeight);
                }
            }
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

                    PinTable[i, j].UpdateHeight(0);
                }
            }

            // await resetBoxDialog.ShowPrompt();
            // Debug.Log("Resetting height");
            // // Generate(rows, columns);

            // // for (int i = 0; i < rows; i++)
            // // {
            // //     for (int j = 0; j < columns; j++)
            // //     {
            // //         ClickedPinTable[i, j] = false;
            // //         PinTable[i, j].Unselect();
            // //         PinTable[i, j].UpdateHeight(0);
            // //     }
            // // }
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

                    PinTable[i, j].UpdateHeight(height);

                }
            }
        }






        #endregion


        #region Height Getter & Setter

        // public flo[,] GetHeights()
        // {
        //     int[,] heights = new int[rows, columns];

        //     for (int i = 0; i < rows; i++)
        //     {
        //         for (int j = 0; j < columns; j++)
        //         {
        //             if (PinTable[i, j] != null)
        //             {
        //                 heights[i, j] = PinTable[i, j].height;
        //             }
        //             else
        //             {
        //                 // If the PinTable[i, j] is null, we assume the height is 0
        //                 heights[i, j] = 0;
        //             }
        //         }
        //     }

        //     return heights;
        // }

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

        #endregion
    }
}

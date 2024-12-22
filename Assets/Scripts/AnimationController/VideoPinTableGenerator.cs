using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Windows.Input;

namespace USPinTable
{
    public class VideoPinTableGenerator : MonoBehaviour
    {

        // public Vector2 pinPos;
        // public RectTransform pinParent;
        // public GameObject pinPrefab;
        // public Prompt resetBoxDialog;
        // public Button confirmResetButton;
        // public int pinWidth = 50; // This is the width for each pin

        public Vector3 cubePos;
        public Transform cubeParent;
        public GameObject cubePrefab;
        public int cubeWidth = 1;
        public int cubeScale = 20;

        public int columns;
        public int rows;

        public VideoSinglePin[,] PinTable;
        private SingleCube[,] CubeTable;
        public bool[,] ClickedPinTable;

        public void Generate(int x, int y)
        {

            ClearCube();

            GenerateCube(x, y);


            // GenerateGrid(x, y);
        }

        private void GenerateCube(int x, int y)
        {
            Vector3 pos = cubeParent.position;
            Debug.Log($"Position of cubeParent: X={pos.x:F2}, Y={pos.y:F2}, Z={pos.z:F2}");
            rows = x;
            columns = y;
            Debug.Log($"Generating cube with {rows} rows and {columns} columns");
            PinTable = new VideoSinglePin[rows, columns];
            CubeTable = new SingleCube[rows, columns];
            ClickedPinTable = new bool[rows, columns]; // Initialize ClickedPinTable

            Vector3 startOffset = new Vector3((cubeWidth * (columns - 1)) / 2.0f, 0, (cubeWidth * (rows - 1)) / 2.0f);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Calculate position for each cube to ensure the table is centered around the parent
                    Vector3 position = new Vector3(j * cubeWidth - startOffset.x, 0, -i * cubeWidth - startOffset.z);
                    // Vector3 position = new Vector3(j * 2, 0, -i * 2);
                    var newCube = Instantiate(cubePrefab, position, Quaternion.identity, transform).transform;
                    newCube.gameObject.layer = 8; // Set the layer to "PinTable" to avoid raycast conflicts
                    newCube.localScale = new Vector3(cubeWidth * 0.99f, cubeWidth * 0.99f, cubeWidth * 0.99f);

                    CubeTable[i, j] = newCube.GetComponent<SingleCube>();
                    CubeTable[i, j].scale = cubeScale;

                    // Add and initialize the VideoSinglePin component
                    var singlePin = newCube.gameObject.AddComponent<VideoSinglePin>();
                    singlePin.cube = CubeTable[i, j];
                    PinTable[i, j] = singlePin;


                    // Debug.Log($"${PinTable[i, j]} created");
                    // Debug.Log($"PinTable[{i},{j}] created and assigned with cube.");
                }
            }
            int midrow = rows / 2;
            int midcol = columns / 2;
            cubeParent.SetPositionAndRotation(CubeTable[midrow, midcol].transform.position, Quaternion.Euler(-77, -32.6f, 0));
            // Increase the y position by 3 units using Vector3.up
            cubeParent.position += 3 * Vector3.up;

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



        #region Update related

        public void ResetHeight()
        {

            Generate(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    ClickedPinTable[i, j] = false;
                    // PinTable[i, j].Unselect();
                    PinTable[i, j].StartHeightTransition(0, 0);
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


        // public void OnPinClicked(int row, int col)
        // {
        //     ClickedPinTable[row, col] = !ClickedPinTable[row, col];
        //     PinTable[row, col].OnClickPin();
        // }

        public void SelectAll()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // PinTable[i, j].Select();
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
                    // PinTable[i, j].Unselect();
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

        public void SetHeights(int[,] heights, float waitTime)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    if (PinTable[i, j] != null)
                    {
                        // Debug.Log($"Setting height to {heights[i, j]}");
                        PinTable[i, j].StartHeightTransition(heights[i, j], waitTime);
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

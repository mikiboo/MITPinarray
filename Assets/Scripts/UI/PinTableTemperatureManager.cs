using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace USPinTable
{
    // [System.Serializable]
    // public class PinTableData
    // {
    //     public int moduleRow = 2;
    //     public int moduleCol = 10;
    //     public int baseRow = 1;
    //     public int baseCol = 1;
    //     public int row = 1;
    //     public int col = 1;
    //     public int totalRows;
    //     public int totalCols;
    //     public int[] serializedPinTable; // Flattened pin table

    //     // Constructor to initialize from existing data
    //     public PinTableData(int[,] pinTable, int baseRow, int baseCol, int row, int col)
    //     {
    //         this.baseRow = baseRow;
    //         this.baseCol = baseCol;
    //         this.row = row;
    //         this.col = col;
    //         // Calculate totalRows and totalCols based on input parameters
    //         this.totalRows = row * this.moduleRow * baseRow;
    //         this.totalCols = col * this.moduleCol * baseCol;
    //         // Convert pinTable from int[,] to List<List<int>>
    //         serializedPinTable = ConvertToSerializableList(pinTable);
    //     }

    //     private int[] ConvertToSerializableList(int[,] pinTable)
    //     {
    //         int[] flattened = new int[pinTable.GetLength(0) * pinTable.GetLength(1)];
    //         for (int i = 0; i < pinTable.GetLength(0); i++)
    //         {
    //             for (int j = 0; j < pinTable.GetLength(1); j++)
    //             {
    //                 flattened[i * pinTable.GetLength(1) + j] = pinTable[i, j];
    //             }
    //         }

    //         return flattened;
    //     }

    //     public int[,] ConvertListTo2DArray()
    //     {
    //         int[,] pinTable = new int[totalRows, totalCols];
    //         for (int i = 0; i < totalRows; i++)
    //         {
    //             for (int j = 0; j < totalCols; j++)
    //             {
    //                 pinTable[i, j] = serializedPinTable[i * totalCols + j];
    //             }
    //         }

    //         return pinTable;
    //     }
    // }

    public class PinTableTemperatureManager : MonoBehaviour
    {
        public PinTableTemperatureGenerator pinTableGenerator;

        // JSON unit: how many number (2*10=20)
        [SerializeField] private int moduleRow = 2; // x
        [SerializeField] private int moduleCol = 10; // y

        // Package unit: how many json (25*2=50)
        [SerializeField] private int baseRow = 1; // x
        [SerializeField] private int baseCol = 1; // y

        // Base unit: how many bases
        [SerializeField] private int row = 1; // x
        [SerializeField] private int col = 1; // y

        [SerializeField] public int totalRows;
        [SerializeField] public int totalCols;

        public int[,] PinTable;

        public TemperatureMonitor temperatureMonitor;

        private string[,] JSONs;
        public string[] Packages;
        void Start()
        {

            InitializePinTable();
        }

        public PinTableTemperatureManager(int baseRow, int baseCol, int row, int col)
        {
            this.row = row;
            this.col = col;
            this.baseRow = baseRow;
            this.baseCol = baseCol;

            InitializePinTable();
        }

        // This method initializes the _pinTable array based on the dimensions.
        public void LoadNewPintableTemperature(string serializedData)
        {
            var deserializedData = JsonUtility.FromJson<PinTableData>(serializedData);


            baseRow = deserializedData.baseRow;
            baseCol = deserializedData.baseCol;
            row = deserializedData.row;
            col = deserializedData.col;

            // Calculate the total dimensions based on the input parameters.
            totalRows = baseRow * row;
            totalCols = baseCol * col;

            GlobalManager.baseRow = baseRow;
            GlobalManager.baseCol = baseCol;
            GlobalManager.row = row;
            GlobalManager.col = col;
            print($"total Rows: {totalRows}");
            print($"total Cols: {totalCols}");
            print($"total Module: {totalRows * totalCols}");

            PinTable = new int[totalRows, totalCols];

            // Optional: Initialize all pins to a default height, e.g., 0
            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < totalCols; j++)
                {
                    PinTable[i, j] = 0; // Default height of pins
                    // PinTable[i, j] = i * totalCols + j * 20; // Debug
                }
            }

            pinTableGenerator.Generate(totalRows, totalCols);
            pinTableGenerator.SetHeights(PinTable);
            temperatureMonitor.resetTemperature();
        }

        public void InitializePinTable()
        {
            moduleRow = GlobalManager.moduleRow;
            moduleCol = GlobalManager.moduleCol;
            baseRow = GlobalManager.baseRow;
            baseCol = GlobalManager.baseCol;
            row = GlobalManager.row;
            col = GlobalManager.col;

            // Calculate the total dimensions based on the input parameters.
            totalRows = baseRow * row;
            totalCols = baseCol * col;
            print($"total Rows: {totalRows}");
            print($"total Cols: {totalCols}");
            print($"total Module: {totalRows * totalCols}");

            PinTable = new int[totalRows, totalCols];

            // Optional: Initialize all pins to a default height, e.g., 0
            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < totalCols; j++)
                {
                    PinTable[i, j] = 0; // Default height of pins
                    // PinTable[i, j] = i * totalCols + j * 20; // Debug
                }
            }

            pinTableGenerator.Generate(totalRows, totalCols);
            pinTableGenerator.SetHeights(PinTable);
        }

        #region Generator for JSONs & Packages

        // public void GenerateJSONs()
        // {
        //     PinTable = pinTableGenerator.GetHeights();

        //     int jsonRows = totalRows / moduleRow;
        //     int jsonCols = totalCols / moduleCol;
        //     JSONs = new string[jsonRows, jsonCols];

        //     int moduleId = 0;
        //     for (int i = 0; i < jsonRows; i++)
        //     {
        //         for (int j = 0; j < jsonCols; j++)
        //         {
        //             int[] heights = new int[moduleRow * moduleCol];
        //             for (int x = 0; x < moduleRow; x++)
        //             {
        //                 for (int y = 0; y < moduleCol; y++)
        //                 {
        //                     heights[x * moduleCol + y] = PinTable[i * moduleRow + x, j * moduleCol + y];
        //                 }
        //             }

        //             // NOTE: moduleId is not correct
        //             JSONs[i, j] = $"{{\"i\":{moduleId},\"s\":1,\"h\":[{string.Join(",", heights)}]}}";
        //             moduleId++;
        //         }
        //     }
        // }

        // public void GeneratePackages()
        // {
        //     GenerateJSONs();
        //     int packageIndex = 0;
        //     int packagesAcross = JSONs.GetLength(1) / baseCol;
        //     Packages = new string[(JSONs.GetLength(0) / baseRow) * packagesAcross];

        //     for (int baseX = 0; baseX < JSONs.GetLength(0); baseX += baseRow)
        //     {
        //         for (int baseY = 0; baseY < JSONs.GetLength(1); baseY += baseCol)
        //         {
        //             List<string> packageJsoNs = new List<string>();
        //             int moduleId = 25; // NOTE: Start from 25, because 1st CAN bus is broken
        //             for (int x = 0; x < baseRow; x++)
        //             {
        //                 for (int y = 0; y < baseCol; y++)
        //                 {
        //                     if (baseX + x < JSONs.GetLength(0) && baseY + y < JSONs.GetLength(1))
        //                     {
        //                         // Update moduleId within JSON string here

        //                         // NOTE: for MIT presentation
        //                         if ((baseRow == 2 && baseCol == 1) || (baseCol == 2 && baseRow == 1))
        //                         {
        //                             string updatedJson = UpdateModuleId(JSONs[baseX + x, baseY + y], moduleId);
        //                             moduleId += 4;
        //                             packageJsoNs.Add(updatedJson);
        //                         }
        //                         else
        //                         {
        //                             string updatedJson = UpdateModuleId(JSONs[baseX + x, baseY + y], moduleId++);
        //                             packageJsoNs.Add(updatedJson);
        //                         }
        //                     }
        //                 }
        //             }

        //             Packages[packageIndex++] = string.Join(";", packageJsoNs);
        //         }
        //     }

        //     // PrintPackages();
        // }

        private string UpdateModuleId(string json, int newId)
        {
            // Assuming the JSON format is as previously defined, replace the moduleId
            // This is a simple string manipulation for demonstration and might need adjustment for complex scenarios
            int index = json.IndexOf("\"i\":", StringComparison.Ordinal) + 4; // Find the index of "i":
            int end = json.IndexOf(",", index, StringComparison.Ordinal); // Find the end of the moduleId value
            string updatedJson = json.Substring(0, index) + newId + json.Substring(end);
            return updatedJson;
        }

        #endregion


        #region Print functions

        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var str = "";
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    str += matrix[i, j] + ",";
                }

                print(str);
            }
        }

        public void PrintPinTable()
        {
            Print2DArray(PinTable);
        }

        // public void PrintJSONs()
        // {
        //     GenerateJSONs();
        //     Print2DArray(JSONs);
        // }

        // public void PrintPackages()
        // {
        //     GeneratePackages();
        //     foreach (var package in Packages)
        //     {
        //         print(package);
        //     }
        // }

        public void PrintClickedPinTable()
        {
            // Print2DArray(pinTableGenerator.ClickedPinTable);
        }

        #endregion


        #region Serialization

        // This method serializes and writes data to a file

        // Serialize the data into a string (you might need to implement this)
        // private string SerializeData()
        // {
        //     PinTable = pinTableGenerator.GetHeights();

        //     PinTableData data = new PinTableData(PinTable, baseRow, baseCol, row, col);
        //     string serializedData = JsonUtility.ToJson(data, true);
        //     print(serializedData);
        //     return serializedData;
        // }

        // This method reads data from a file and deserializes it to re-initialize the object
        // public void LoadDataFromFile(string path)
        // {
        //     if (File.Exists(path))
        //     {
        //         // Read the data from the file
        //         string serializedData = File.ReadAllText(path);

        //         // Deserialize the data to re-initialize the object
        //         DeserializeData(serializedData);
        //     }
        //     else
        //     {
        //         Debug.LogError("File not found");
        //     }
        // }

        // Deserialize the data (you might need to adjust this method to fit your needs)
        // private void DeserializeData(string serializedData)
        // {
        //     print(serializedData);
        //     var deserializedData = JsonUtility.FromJson<PinTableData>(serializedData);

        //     moduleRow = deserializedData.moduleRow;
        //     moduleCol = deserializedData.moduleCol;
        //     baseRow = deserializedData.baseRow;
        //     baseCol = deserializedData.baseCol;
        //     row = deserializedData.row;
        //     col = deserializedData.col;
        //     totalRows = deserializedData.totalRows;
        //     totalCols = deserializedData.totalCols;
        //     GenerateJSONs();
        //     GeneratePackages();
        //     PinTable = deserializedData.ConvertListTo2DArray();

        //     pinTableGenerator.SetHeights(PinTable);
        // }

        #endregion
    }
}

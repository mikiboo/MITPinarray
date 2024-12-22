using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PinTableTranslator
{
    // How many pins in one module? (fixed => 2 * 10)
    public int moduleRow = 2;
    public int moduleCol = 10;

    // How many modules in one base? (fixed => 5 * 1)
    public int baseRow = 5;
    public int baseCol = 1;

    // How many bases in the whole PinTable? (flexible)
    public int row = 1;
    public int col = 1;

    private int totalRows;
    private int totalCols;

    // public int[] serializedPinTable; // Flattened pin table

    public int[,] PinTable;

    private string[,] JSONs;
    public string[] Packages;

    public PinTableTranslator(int row, int col)
    {
        this.row = row;
        this.col = col;

        totalRows = row * moduleRow * baseRow;
        totalCols = col * moduleCol * baseCol;

        PinTable = new int[totalRows, totalCols];

        Debug.Log($"Init: Empty PinTable[{totalRows},{totalCols}]");
    }

    public string[] Translate(string JSONPayload)
    {
        #region Perceive data from JSON payload

        // Deserialize JSONPayload into an object
        var pinData = JsonConvert.DeserializeObject<PinData>(JSONPayload);

        // Check if the payload exceeds the capacity of the PinTable
        if (pinData.pins.Length > totalRows * totalCols)
        {
            throw new Exception("Payload exceeds the capacity of the PinTable.");
        }

        PinTable = new int[totalRows, totalCols];

        // Populate the PinTable
        foreach (var pin in pinData.pins)
        {
            var indices = pin.id.Split('.');
            if (indices.Length != 2 || !int.TryParse(indices[0], out int rowIndex) ||
                !int.TryParse(indices[1], out int colIndex))
            {
                throw new ArgumentException("Position must be in the format 'rowIndex.colIndex' with valid integers.");
            }

            // Ensure the indices are within the bounds of the table
            if (rowIndex >= totalRows || colIndex >= totalCols)
            {
                throw new ArgumentOutOfRangeException(
                    "Position indices are out of the range of the PinTable dimensions.");
            }

            // Extract the pin number from the id and assign it to the table
            PinTable[rowIndex, colIndex] = pin.position;
        }

        Print2DArray(PinTable);

        #endregion

        GeneratePackages();

        foreach (var package in Packages)
        {
            Debug.Log(package);
        }

        return Packages;
    }

    private void GeneratePackages()
    {
        GenerateJSONs();
        int packageIndex = 0;
        int packagesAcross = JSONs.GetLength(1) / baseCol;
        Packages = new string[(JSONs.GetLength(0) / baseRow) * packagesAcross];

        for (int baseX = 0; baseX < JSONs.GetLength(0); baseX += baseRow)
        {
            for (int baseY = 0; baseY < JSONs.GetLength(1); baseY += baseCol)
            {
                List<string> packageJsoNs = new List<string>();
                int moduleId = 0;
                for (int x = 0; x < baseRow; x++)
                {
                    for (int y = 0; y < baseCol; y++)
                    {
                        if (baseX + x < JSONs.GetLength(0) && baseY + y < JSONs.GetLength(1))
                        {
                            // Update moduleId within JSON string here
                            string updatedJson = UpdateModuleId(JSONs[baseX + x, baseY + y], moduleId++);
                            packageJsoNs.Add(updatedJson);
                        }
                    }
                }

                Packages[packageIndex++] = string.Join(";", packageJsoNs);
            }
        }

        // PrintPackages();
    }

    private string UpdateModuleId(string json, int newId)
    {
        // Assuming the JSON format is as previously defined, replace the moduleId
        // This is a simple string manipulation for demonstration and might need adjustment for complex scenarios
        int index = json.IndexOf("\"i\":", StringComparison.Ordinal) + 4; // Find the index of "i":
        int end = json.IndexOf(",", index, StringComparison.Ordinal); // Find the end of the moduleId value
        string updatedJson = json.Substring(0, index) + newId + json.Substring(end);
        return updatedJson;
    }

    private void GenerateJSONs()
    {
        int jsonRows = totalRows / moduleRow;
        int jsonCols = totalCols / moduleCol;
        JSONs = new string[jsonRows, jsonCols];

        int moduleId = 0;
        for (int i = 0; i < jsonRows; i++)
        {
            for (int j = 0; j < jsonCols; j++)
            {
                int[] heights = new int[moduleRow * moduleCol];
                for (int x = 0; x < moduleRow; x++)
                {
                    for (int y = 0; y < moduleCol; y++)
                    {
                        heights[x * moduleCol + y] = PinTable[i * moduleRow + x, j * moduleCol + y];
                    }
                }

                // NOTE: moduleId is not correct
                JSONs[i, j] = $"{{\"i\":{moduleId},\"s\":1,\"h\":[{string.Join(",", heights)}]}}";
                moduleId++;
            }
        }

        Print2DArray<string>(JSONs);
    }

    #region debug

    // for testing, baseRow and baseCol are not fixed
    public PinTableTranslator(int baseRow, int baseCol, int row, int col)
    {
        this.baseRow = baseRow;
        this.baseCol = baseCol;
        this.row = row;
        this.col = col;

        totalRows = row * moduleRow * baseRow;
        totalCols = col * moduleCol * baseCol;

        PinTable = new int[totalRows, totalCols];

        Debug.Log($"Init: Empty PinTable[{totalRows},{totalCols}]");
    }

    private static void Print2DArray<T>(T[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            var str = "";
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                str += matrix[i, j] + ",";
            }

            Debug.Log(str);
        }
    }

    #endregion
}

// Helper class to deserialize JSON data
public class PinData
{
    public Pin[] pins { get; set; }
}

public class Pin
{
    public string id { get; set; }
    public int position { get; set; }
}

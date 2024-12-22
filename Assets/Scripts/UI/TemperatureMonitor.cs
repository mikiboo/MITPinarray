using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USPinTable;

public class TemperatureMonitor : MonoBehaviour
{
    public PinTableTemperatureGenerator pinTableGenerator;
    public GameObject temperatureAlert;
    private float[,] currentTemperatures;
    private float[,] targetTemperatures;
    private float temperatureChangeRate = 0.1f; // Change this to control how fast the temperature changes
    private HashSet<(int, int)> highTemperaturePins = new HashSet<(int, int)>();
    int rows;
    int columns;
    private void Start()
    {
        rows = GlobalManager.baseRow * GlobalManager.row;
        columns = GlobalManager.baseCol * GlobalManager.col;

        // Initialize temperature arrays
        currentTemperatures = new float[rows, columns];
        targetTemperatures = new float[rows, columns];
        temperatureAlert.SetActive(false);

        // Set initial target temperatures
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                SetNewTargetTemperature(i, j);
            }
        }
    }
    public void resetTemperature()
    {
        rows = GlobalManager.baseRow * GlobalManager.row;
        columns = GlobalManager.baseCol * GlobalManager.col;
        currentTemperatures = new float[rows, columns];
        targetTemperatures = new float[rows, columns];
        temperatureAlert.SetActive(false);

        // Set initial target temperatures
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                SetNewTargetTemperature(i, j);
            }
        }
    }
    private void Update()
    {


        // Update each pin's temperature
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Smoothly interpolate towards the target temperature
                currentTemperatures[i, j] = Mathf.Lerp(currentTemperatures[i, j], targetTemperatures[i, j], temperatureChangeRate * Time.deltaTime);
                float integerTemperature = (float)System.Math.Round(currentTemperatures[i, j], 1);

                // Update the pin table generator with the new temperature value
                pinTableGenerator.PinTable[i, j].UpdateHeight(integerTemperature);

                // Check if the temperature exceeds the threshold and handle the high temperature list
                if (integerTemperature >= 90)
                {
                    highTemperaturePins.Add((i, j));
                }
                else
                {
                    highTemperaturePins.Remove((i, j));
                }

                // Check if the current temperature is close enough to the target temperature to set a new target
                if (Mathf.Abs(currentTemperatures[i, j] - targetTemperatures[i, j]) < 0.1f)
                {
                    SetNewTargetTemperature(i, j);
                }
            }
        }

        // Set the alert based on the high temperature pins count
        temperatureAlert.SetActive(highTemperaturePins.Count > 0);
    }

    private void SetNewTargetTemperature(int row, int column)
    {
        targetTemperatures[row, column] = Random.Range(0, 100);
    }
}

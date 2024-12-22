using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using USPinTable;

public class VideoPinTableConnector : MonoBehaviour
{
    private Dictionary<int, string> deviceMap = new Dictionary<int, string>(); // Map to hold device ID and IP

    public bool debug = true;

    public bool reset = false;

    public void Start()
    {
        InitializeWinsock(GlobalManager.row * GlobalManager.col);
        if (reset)
        {
            print("Resetting pin-array");
            // SendPackages();
        }
    }

    public void InitializeWinsock(int num)
    {
        for (int i = 0; i < num; i++)
        {
            string ip;
            if (debug)
            {
                ip = $"127.0.0.1";
            }
            else
            {
                ip = $"192.168.0.{i + 10}";
            }

            deviceMap[i] = ip;
            print($"Init Device[{i}]: {ip}");
        }
    }

    public async Task SendMessageToIDAsync(int id, string jsonMessage)
    {
        if (!deviceMap.ContainsKey(id))
        {
            Debug.LogError("Device ID not found.");
        }

        try
        {
            var ip = deviceMap[id];
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(IPAddress.Parse(ip), 5000); // Asynchronously connect to the server
                Debug.Log("Connected to " + ip);

                // Get a stream object for writing and reading
                NetworkStream stream = client.GetStream();

                // Send the message to the connected server
                byte[] bytesToSend = Encoding.ASCII.GetBytes(jsonMessage + Environment.NewLine);
                await stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                Debug.Log("Message sent");
            }

            Debug.Log("Disconnected with " + ip);
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to connect or send: {e.Message}");
            //  Debug.LogError($"Failed to connect or send: {e.Message}");
        }
    }

    // Coroutine wrapper to call SendMessageToIDAsync
    private IEnumerator SendMessageToIDCoroutine(int id, string jsonMessage)
    {
        bool messageSent = false;
        string response = "";

        // Start the async method
        Task task = SendMessageToIDAsync(id, jsonMessage);
        task.ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                Debug.Log($"Message sent successfully");
            }
            else if (t.IsFaulted)
            {
                Debug.LogError($"Failed to send message: {t.Exception}");
            }

            messageSent = true; // Signal that the task is completed
        });

        // Wait until the task is completed
        yield return new WaitUntil(() => messageSent);
        // Optionally use the response here
    }

    public VideoPinTableManager manager;

    // Example usage methods
    public void Connect(string serializedData)
    {
        // Assuming manager.GeneratePackages() populates manager.Packages with the necessary data
        manager.GeneratePackages(serializedData);
        InitializeWinsock(manager.Packages.GetLength(0));
    }

    public IEnumerator SendPackages(string serializedData)
    {
        var deserializedData = JsonUtility.FromJson<VideoPinTableData>(serializedData);
        InitializeWinsock(deserializedData.totalRows * deserializedData.totalCols);
        yield return StartCoroutine(manager.GeneratePackages(serializedData));
        // Debug.Log("testing");
        // Debug.Log("manager.Packages.GetLength(0): " + manager.Packages.GetLength(0));
        for (var i = 0; i < manager.Packages.GetLength(0); i++)
        {
            Debug.Log($"Send to Device[{i}] - {deviceMap[i]}\nMessage: {manager.Packages[i]}");
            yield return StartCoroutine(SendMessageToIDCoroutine(i, manager.Packages[i]));
        }
    }
}

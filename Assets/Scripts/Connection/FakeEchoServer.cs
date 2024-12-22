using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FakeEchoServer : MonoBehaviour
{
    private const int Port = 5000;

    // Start the server as soon as the game starts
    void Start()
    {
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        // Start the server
        Task.Run(() => StartServerAsync(localAddr, Port));
    }

    private async Task StartServerAsync(IPAddress ipAddress, int port)
    {
        var listener = new TcpListener(ipAddress, port);

        try
        {
            Debug.Log($"Server tried at {ipAddress}:{port}");
            listener.Start();
            Debug.Log($"Server started at {ipAddress}:{port}");

            while (true)
            {
                Debug.Log("Waiting for connection...");
                var client = await listener.AcceptTcpClientAsync();
                Debug.Log("Connected!");

                // Handle the client in a new task
                Task.Run(() => HandleClientAsync(client));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Server error: {e.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int numberOfBytesRead;

                while ((numberOfBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    var msg = Encoding.ASCII.GetString(buffer, 0, numberOfBytesRead);
                    Debug.Log($"Received: {msg}");

                    // Echo the message back
                    await stream.WriteAsync(buffer, 0, numberOfBytesRead);
                    Debug.Log($"Echoed: {msg}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Client handling error: {e.Message}");
        }
    }
}

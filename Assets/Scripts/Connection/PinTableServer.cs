using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PinTableServer : MonoBehaviour
{
    public bool listenAtAll = true; // For debug
    public int portNum = 8052;

    public AddressType addrType = AddressType.IPv4;

    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    // Start is called before the first frame update
    void Start()
    {
        // Start TcpServer background thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private void ListenForIncomingRequests()
    {
        try
        {
            IPAddress addr = IPAddress.Parse(DeviceTool.GetIPbyType(addrType));
            if (listenAtAll)
            {
                tcpListener = new TcpListener(IPAddress.Any, portNum);
                tcpListener.Start();
                Debug.Log($"Server is listening at all internet interfaces");
            }
            else
            {
                tcpListener = new TcpListener(addr, portNum);
                tcpListener.Start();
                Debug.Log($"Server is listening at {addr}:{portNum}");
            }

            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            string clientMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("client message received as: " + clientMessage);

                            // Process JSON data here
                            // For example: JsonConvert.DeserializeObject<MyDataType>(clientMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.LogError("SocketException " + socketException.ToString());
        }
    }

    private void OnDestroy()
    {
        // Stop listening thread
        if (tcpListenerThread != null)
        {
            tcpListenerThread.Abort();
        }

        // Close the listener and the client
        tcpListener?.Stop();
        connectedTcpClient?.Close();
    }
}

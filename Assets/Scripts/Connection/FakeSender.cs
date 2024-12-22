using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Mono.WebBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FakeSender : MonoBehaviour
{
    public TMP_InputField inputField;
    public int serverPort = 8052;

    public AddressType addrType = AddressType.IPv4;

    public void SendInputToServer()
    {
        if (inputField.text.Length == 0)
        {
            Debug.Log("Input field is empty.");
            return;
        }

        try
        {
            using (TcpClient client = new TcpClient(DeviceTool.GetIPbyType(addrType), serverPort))
            {
                int byteCount = Encoding.ASCII.GetByteCount(inputField.text + 1);
                byte[] sendData = new byte[byteCount];

                sendData = Encoding.ASCII.GetBytes(inputField.text + "\n");

                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
                Debug.Log("Data sent: " + inputField.text);
                stream.Close();
            }
        }
        catch (Exception socketException)
        {
            Debug.LogError("Socket exception: " + socketException);
        }
    }
}

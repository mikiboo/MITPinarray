using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public class DeviceTool
{
    #region Device Information

    public static string GetDeviceName()
    {
        return UnityEngine.SystemInfo.deviceName;
    }

    public static string GetDeviceModel()
    {
        return UnityEngine.SystemInfo.deviceModel;
    }

    public static string GetDeviceUniqueIdentifier()
    {
        return UnityEngine.SystemInfo.deviceUniqueIdentifier;
    }

    #endregion

    public static string GetLocalIp(AddressType addressType)
    {
        if (addressType == AddressType.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = string.Empty;

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) &&
                item.OperationalStatus == OperationalStatus.Up)
#endif
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (addressType == AddressType.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                    else if (addressType == AddressType.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }

        return output;
    }

    public static string GetExtranetIp()
    {
        string IP = string.Empty;
        try
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = System.Text.Encoding.Default;
            IP = client.DownloadString("http://checkip.amazonaws.com/");
            client.Dispose();
            IP = Regex.Replace(IP, @"[\r\n]", "");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return IP;
    }

    public static string GetIPbyType(AddressType type)
    {
        if (type == AddressType.PublicIP)
        {
            return GetExtranetIp();
        }

        return GetLocalIp(type);
    }

    public static List<string> GetMACList(OperationalStatus operationalStatus = OperationalStatus.Up,
        bool isAppendName = true)
    {
        var list = new List<string>();
        NetworkInterface[] allNetWork = NetworkInterface.GetAllNetworkInterfaces();
        if (allNetWork.Length > 0)
        {
            foreach (var item in allNetWork)
            {
                if (item.OperationalStatus == operationalStatus)
                {
                    string strInfo = isAppendName
                        ? item.GetPhysicalAddress().ToString() + $"({item.Name})"
                        : item.GetPhysicalAddress().ToString();
                    list.Add(strInfo);
                }
            }
        }
        else
        {
            Console.WriteLine("No network interface");
        }

        return list;
    }
}

public enum AddressType
{
    IPv4,
    IPv6,
    PublicIP,
}

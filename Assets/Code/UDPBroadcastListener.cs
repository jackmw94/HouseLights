using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class UDPBroadcastListener : MonoBehaviour
{
    public struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
    }

    [SerializeField] private int listenPort = 8888;
    [SerializeField] private List<EndPoint> receivedTransmissions;

    public static bool messageReceived = false;
    private static WaitForSeconds waitForSecond = new WaitForSeconds(1f);

    private IEnumerator Start()
    {
        yield return ReceiveMessages(listenPort);
    }

    public static void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] receiveBytes = u.EndReceive(ar, ref e);
        string receiveString = Encoding.ASCII.GetString(receiveBytes);

        print($"Received: {receiveString} from {e.Address}:{e.Port}");
        messageReceived = true;
    }

    public static IEnumerator ReceiveMessages(int listenPort)
    {
        // Receive a message and write it to the console.
        var localAddress = GetLocalIPAddress().GetAddressBytes();
        IPEndPoint e = new IPEndPoint(IPAddress.Any, listenPort);
        UdpClient u = new UdpClient(e);
        u.EnableBroadcast = true;

        UdpState s = new UdpState();
        s.e = e;
        s.u = u;

        print("listening for messages");
        u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

        // Do some work while we wait for a message. For this example, we'll just sleep
        while (!messageReceived)
        {
            yield return waitForSecond;
        }
    }

    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
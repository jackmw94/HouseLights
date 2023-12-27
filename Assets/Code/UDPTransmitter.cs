using System.Net;
using System.Net.Sockets;
using UnityEngine;
public class UDPTransmitter : LEDTransmitter
{
    [SerializeField] EndPoint endPoint;

    UdpClient client;

    [ContextMenu("Reinit web socket")]
    private void Start()
    {
        Initialise();
    }

    private async void Initialise()
    {
        client = new UdpClient(endPoint.port);
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(endPoint.url), endPoint.port);
        client.Connect(ep);
    }

    public override void Write(byte[] data, int count)
    {
        client.Send(data, count);
    }
}

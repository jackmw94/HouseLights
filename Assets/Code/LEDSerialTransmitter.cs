using System.IO.Ports;
using UnityEngine;

public class LEDSerialTransmitter : LEDTransmitter
{
    [SerializeField] private string serialPortName = "COM3";
    [SerializeField] private int baud = 115200;
    [Space]
    [SerializeField] private bool verbose;

    private SerialPort stream;

    private void Start()
    {
        stream = new SerialPort(serialPortName, baud);
        stream.Open();
    }

    private void OnDestroy()
    {
        if (stream != null)
        {
            stream.Close();
        }
    }

    public override void Write(byte[] data, int count)
    {
        if (verbose) Debug.Log($"Writing {count} bytes");
        stream.Write(data, 0, count);
    }
}

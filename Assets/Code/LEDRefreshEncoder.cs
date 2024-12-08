using System.Collections.Generic;
using UnityEngine;

public class LEDRefreshEncoder : LEDEncoder
{
    private const int PacketSize = 2;

    [SerializeField] private LEDTransmitter ledTransmitter;
    [Space]
    [SerializeField] private int sendDataEveryNFrames = 5;
    
    private readonly byte[] writeBuffer = new byte[500 * PacketSize];

    private void LateUpdate()
    {
        if (Time.frameCount % sendDataEveryNFrames != 0)
        {
            return;
        }

        ledTransmitter.Write(writeBuffer, writeBuffer.Length);
    }

    public override void UpdateLED(int index, Color colour)
    {
        (byte x1, byte x2) = LEDEncodingHelper.GetEncodedDataFromChange(index, colour);

        int arrayOffset = index * PacketSize;
        writeBuffer[arrayOffset + 0] = x1;
        writeBuffer[arrayOffset + 1] = x2;
    }
}

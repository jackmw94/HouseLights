using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;

public class LEDChangeEncoder : LEDEncoder
{
    private struct LEDChange
    {
        public int index;
        public Color colour;
        public int frame;
    }

    private const int PacketSize = 2;

    [SerializeField] private LEDTransmitter ledTransmitter;
    [Space]
    [SerializeField] private int sendDataEveryNFrames = 1;
    [SerializeField] private int maxUpdatesPerFrame = 50;
    [Space]
    [SerializeField] private bool verbose;

    int maxSimultaneousChangeCount = 0;

    private readonly Dictionary<int,LEDChange> changes = new();
    private readonly Dictionary<int, byte> cachedSent = new();
    private readonly List<int> toRemove = new();

    private readonly byte[] writeBuffer = new byte[400 * PacketSize];

    private void LateUpdate()
    {
        if (changes.Count == 0 || Time.frameCount % sendDataEveryNFrames != 0)
        {
            return;
        }

        toRemove.Clear();

        IOrderedEnumerable<LEDChange> ordered = changes.Values.OrderBy(p => p.frame);

        int changeCount = 0;
        foreach(var change in ordered)
        {
            if (TryAddChangeToArray(changeCount, change))
            {
                changeCount++;
            }

            toRemove.Add(change.index);

            if (changeCount >= maxUpdatesPerFrame)
            {
                break;
            }
        }

        foreach(int remove in toRemove)
        {
            changes.Remove(remove);
        }

        ledTransmitter.Write(writeBuffer, changeCount * PacketSize);

        maxSimultaneousChangeCount = Mathf.Max(maxSimultaneousChangeCount, changeCount);
        if (verbose) Debug.Log($"[Frame {Time.frameCount}] Sent {changeCount} changes (max={maxSimultaneousChangeCount}). {changes.Count} remaining changes");
    }

    public override void UpdateLED(int index, Color colour)
    {
        changes[index] = new LEDChange()
        {
            index = index,
            colour = colour,
            frame = Time.frameCount
        };
    }

    private bool TryAddChangeToArray(int changeCount, LEDChange ledChange)
    {
        int arrayOffset = changeCount * PacketSize;

        (byte x1, byte x2) = LEDEncodingHelper.GetEncodedDataFromChange(ledChange.index, ledChange.colour);

        writeBuffer[arrayOffset + 0] = x1;
        writeBuffer[arrayOffset + 1] = x2;

        if (cachedSent.TryGetValue(ledChange.index, out var cachedVal) && cachedVal == x2)
        {
            // encoded data is same as cached
            return false;
        }

        cachedSent[ledChange.index] = x2;

        if (verbose)
        {
            Color reconstructed = LEDEncodingHelper.ReconstructColourFromEncoding(x1, x2);
            string colourString = ColorUtility.ToHtmlStringRGB(reconstructed);
            Debug.Log($"<color=#{colourString}>[{colourString}] Sent update: {writeBuffer[arrayOffset + 0]}, {writeBuffer[arrayOffset + 1]} to change led at index {ledChange.index}</color>");
        }

        return true;
    }   
}

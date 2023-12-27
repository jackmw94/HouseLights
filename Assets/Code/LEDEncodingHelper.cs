using UnityEngine;

public static class LEDEncodingHelper
{
    public static (byte, byte) GetEncodedDataFromChange(int index, Color colour)
    {
        byte x1 = (byte)(index & 0xff);
        byte x2 = (byte)((index >> 8) & 0x03);

        int intR = (int)Mathf.Clamp(Mathf.Floor(colour.r * 4f), 0, 3);
        byte encodedR = (byte)(intR & 0x03);
        byte shiftedR = (byte)(encodedR << 6);

        int intG = (int)Mathf.Clamp(Mathf.Floor(colour.g * 4f), 0, 3);
        byte encodedG = (byte)(intG & 0x03);
        byte shiftedG = (byte)(encodedG << 4);

        int intB = (int)Mathf.Clamp(Mathf.Floor(colour.b * 4f), 0, 3);
        byte encodedB = (byte)(intB & 0x03);
        byte shiftedB = (byte)(encodedB << 2);

        x2 |= shiftedR;
        x2 |= shiftedG;
        x2 |= shiftedB;

        return (x1, x2);
    }

    public static Color ReconstructColourFromEncoding(byte x1, byte x2)
    {
        int r1 = x2 & 0xC0;
        int r2 = (r1 >> 6);
        float r3 = r2 / 3f;
        int r = Mathf.FloorToInt(r3 * 255);

        int g1 = x2 & 0x30;
        int g2 = (g1 >> 4);
        float g3 = g2 / 3f;
        int g = Mathf.FloorToInt(g3 * 255);

        int b1 = x2 & 0x0C;
        int b2 = (b1 >> 2);
        float b3 = b2 / 3f;
        int b = Mathf.FloorToInt(b3 * 255);

        Color reconstructed = new Color(r / 255f, g / 255f, b / 255f);
        return reconstructed;
    }
}
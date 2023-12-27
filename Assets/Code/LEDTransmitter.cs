using UnityEngine;

public abstract class LEDTransmitter : MonoBehaviour
{
    public abstract void Write(byte[] data, int count);
}

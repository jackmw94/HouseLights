using System;
using UnityEngine;

[Serializable]
internal class LEDSection
{
    [SerializeField] private int fromIndex;
    [SerializeField] private int toIndex;
    [SerializeField] private LEDEncoder encoder;

    public int From => fromIndex;
    public int To => toIndex;
        
    public bool TryUpdateLED(int index, Color colour)
    {
        if (!DoesSectionHandleIndex(index))
        {
            return false;
        }

        encoder.UpdateLED(index - fromIndex, colour);
        return true;
    }

    private bool DoesSectionHandleIndex(int index)
    {
        return index >= fromIndex && index < toIndex;
    }
}
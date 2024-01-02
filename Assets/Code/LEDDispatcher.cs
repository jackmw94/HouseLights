using System.Collections.Generic;
using UnityEngine;

public partial class LEDDispatcher : MonoBehaviour
{

    [SerializeField] private LEDSection[] sections;

    public void UpdateLED(int index, Color colour)
    {
        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i].TryUpdateLED(index, colour))
            {
                return;
            }
        }

        Debug.LogError($"No section handles led at index {index}");
    }
}
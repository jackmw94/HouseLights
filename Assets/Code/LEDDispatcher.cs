using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LEDDispatcher : MonoBehaviour
{
    [SerializeField] private LEDSection[] sections;

    private void OnEnable()
    {
        ResetAll();
    }

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

    [Button]
    public void ResetAll()
    {
        foreach (LEDSection section in sections)
        {
            for (int i = section.From; i < section.To; i++)
            {
                section.TryUpdateLED(i, Color.black);
            }
        }
    }
}
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LiveText : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private TMP_Text label;

    private static readonly KeyCode[] keyCodes = Enum.GetValues(typeof(KeyCode))
                                                 .Cast<KeyCode>()
                                                 .Where(k => ((int)k < (int)KeyCode.Mouse0))
                                                 .ToArray();

    private void Update()
    {
        var currentKey = GetCurrentKeyDown();
        if (currentKey.HasValue)
        {
            label.text = currentKey.Value == KeyCode.Escape ? "" : currentKey.Value.ToString();
        }
    }

    

    private static KeyCode? GetCurrentKeyDown()
    {
        if (!Input.anyKey)
        {
            return null;
        }

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                return keyCodes[i];
            }
        }
        return null;
    }
}

using TMPro;
using UnityEngine;

public class SetText : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private int character;
    [SerializeField] private TMP_Text label;
    [SerializeField] private float showDuration;
    [Space]
    [SerializeField] private int count;
    [SerializeField] private string currentChar;

    private int prevCharacter;
    private float prevChangeTime = 0;

    private void OnValidate()
    {
        count = text.Length;
        currentChar = text.Substring(character, 1);
    }

    private void Update()
    {
        if (prevCharacter != character)
        {

            label.text = text.Substring(character, 1);
            prevCharacter = character;
            prevChangeTime = Time.time;
        }
    }
}

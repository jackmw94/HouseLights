using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SetText : TimedEffect
{
    [SerializeField] private string text;
    [SerializeField] private int character;
    [SerializeField] private TMP_Text label;
    [SerializeField] private float showDuration;
    [SerializeField] private float spaceDuration;
    [SerializeField] private float interDuration;
    [SerializeField] private float rerunPauseDuration;
    [Space]
    [SerializeField] private int count;
    [SerializeField] private string currentChar;
    
    private void OnValidate()
    {
        count = text.Length;
        currentChar = text.Substring(character, 1);
        gameObject.name = text;
    }

    private void OnEnable()
    {
        StartCoroutine(RunEffect());
    }

    private IEnumerator RunEffect()
    {
        while (true)
        {
            IsFinished = false;
            string currentText = text.Substring(character, 1);
            label.text = currentText;

            bool isSpace = currentText.Equals(" ");
            float duration = isSpace ? spaceDuration : showDuration;
            yield return new WaitForSeconds(duration);
            
            character++;

            if (character < text.Length)
            {
                string nextText = text.Substring(character, 1);
                if (nextText.Equals(currentText))
                {
                    label.text = "";
                    yield return new WaitForSeconds(interDuration);
                }
            }
            else
            {
                character = 0;
                label.text = "";
                
                yield return new WaitForSeconds(rerunPauseDuration / 2f);
                IsFinished = true;
                yield return new WaitForSeconds(rerunPauseDuration / 2f);
            }

            yield return null;
        }
    }
}

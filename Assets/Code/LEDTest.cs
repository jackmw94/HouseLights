using UnityEngine;
using System.Collections;

public class LEDTest : MonoBehaviour
{
    [SerializeField] private LEDDispatcher dispatcher;
    [Space]
    [SerializeField] private int ledCount = 400;
    [SerializeField, Range(0,800)] private int ledId;
    [SerializeField] private Color colour;
    [Space]
    [SerializeField] private float testDelay = 0.05f;
    [SerializeField] private float testColorChange = 0.5f;

    private bool stopLoop;

    [ContextMenu(nameof(Send))]
    private void Send()
    {
        dispatcher.UpdateLED(ledId, colour);
    }

    [ContextMenu(nameof(ResetAll))]
    public void ResetAll()
    {
        for (int i = 0; i < ledCount; i++)
        {
            dispatcher.UpdateLED(i, Color.black);
        }
    }

    [ContextMenu(nameof(RunStrip))]
    private void RunStrip()
    {
        StartCoroutine(RunStripInternal());
    }

    private IEnumerator RunStripInternal()
    {
        WaitForSeconds wait = new WaitForSeconds(testDelay);
        for (int i = 0; i < ledCount; i++)
        {
            dispatcher.UpdateLED(i, colour);
            yield return wait;
        }
    }

    [ContextMenu(nameof(RunLoop))]
    private void RunLoop()
    {
        stopLoop = false;
        StartCoroutine(RunLoopInternal());
    }

    [ContextMenu(nameof(StopLoop))]
    private void StopLoop()
    {
        StopAllCoroutines();
    }

    private IEnumerator RunLoopInternal()
    {
        WaitForSeconds wait = new WaitForSeconds(testDelay);
        while (true)
        {           
            for (int i = 0; i < ledCount; i++)
            {
                Color color = Color.HSVToRGB(((Time.time * testColorChange) + (i / (float)ledCount)) % 1, 1f, 1f);
                dispatcher.UpdateLED(i, color);
            }

            yield return wait;
            if (stopLoop) yield break;
        }
    }
}

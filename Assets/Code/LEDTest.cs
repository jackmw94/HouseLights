using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class LEDTest : MonoBehaviour
{
    [SerializeField] private LEDEncoder ledCommunication;
    [Space]
    [SerializeField, Range(0,400)] private int index;
    [SerializeField] private Color colour;
    [Space]
    [SerializeField] private float testDelay = 0.05f;
    [SerializeField] private float testColorChange = 0.5f;

    private bool stopLoop;

    [ContextMenu(nameof(Send))]
    private void Send()
    {
        ledCommunication.UpdateLED(index, colour);
    }

    [ContextMenu(nameof(ResetAll))]
    private void ResetAll()
    {
        for (int i = 0; i < 400; i++)
        {
            ledCommunication.UpdateLED(i, Color.black);
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
        for (int i = 0; i < 400; i++)
        {
            ledCommunication.UpdateLED(i, colour);
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
            for (int i = 0; i < 400; i++)
            {
                Color color = Color.HSVToRGB(((Time.time * testColorChange) + (i / 400f)) % 1, 1f, 1f);
                ledCommunication.UpdateLED(i, color);
            }

            yield return wait;
            if (stopLoop) yield break;
        }
    }
}

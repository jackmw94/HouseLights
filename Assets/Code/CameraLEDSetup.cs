using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraLEDSetup : MonoBehaviour
{
    private enum SetupMode
    {
        Initial,
        Revision
    }

    [SerializeField] private RenderTexture setupRT;
    [SerializeField] private LEDDispatcher dispatcher;
    [Space]
    [SerializeField] private int ledStartId = 0;
    [SerializeField] private int ledCount = 100;
    [SerializeField] private LEDData ledData;
    [Space]
    [SerializeField] private bool waitForUserInput = false;
    [SerializeField] private float agreementThreshold = 0.01f;
    [SerializeField] private int queueSize = 20;
    [SerializeField, Range(0f, 1f)] private float colourStrength = 1f;
    [SerializeField] private float enableLedDelay = 0.2f;
    [SerializeField] private float updateDelay = 0.1f;
    [SerializeField] private float currentGizmoSize = 0.1f;
    [SerializeField] private float averageGizmoSize = 0.1f;
    [SerializeField] private float storedGizmoSize = 0.1f;

    private Vector2[] backupPositions;
    private Vector2[] positions;
    private readonly Queue<Vector2> queuedGuesses = new();

    private int currentLED = -1;
    private int index = 0;
    private Vector2 currentPosition;
    private Vector2 averagePosition;

    Texture2D cachedTexture_BackingField;
    Texture2D CachedTexture => cachedTexture_BackingField ?? (cachedTexture_BackingField = new Texture2D(setupRT.width, setupRT.height));

    private IEnumerator Start()
    {
        ledData.Backup();
        positions = new Vector2[ledCount];
        for (int i = 0; i < Mathf.Min(ledCount, ledData.LightCount); i++)
        {
            positions[i] = ledData.GetPosition(i);
        }

        AdjustIndex(ledStartId);
        yield return RefreshCurrentLED();

        bool next = false;
        bool previous = false;
        bool update = false;
        bool save = false;       
        bool revertToBackup = false;       

        while (true)
        {
            if (update)
            {
                positions[index] = averagePosition;
                update = false;
            }

            if (save)
            {
                ledData.SetLEDPositions(positions);
#if UNITY_EDITOR
                EditorUtility.SetDirty(ledData);
#endif
                save = false;
            }

            if (revertToBackup)
            {
                ledData.LoadBackup();
#if UNITY_EDITOR
                EditorUtility.SetDirty(ledData);
#endif

                revertToBackup = false;
            }

            if (next || previous)
            {
                Debug.Assert(!(next && previous), "Trying to go back and forward at same time");
                AdjustIndex(next ? 1 : -1);

                yield return RefreshCurrentLED();

                queuedGuesses.Clear();

                next = false;
                previous = false;
            }

            currentPosition = GetPositionFromTexture();
            UpdateAverage();

            float delayStart = Time.time;
            while (Time.time - delayStart < updateDelay)
            {
                bool autoAdvance = !waitForUserInput && IsBelowAgreementThreshold();
                bool inputAdvance = Input.GetKeyDown(KeyCode.RightArrow);
                next |= autoAdvance || inputAdvance;

                previous |= Input.GetKeyDown(KeyCode.LeftArrow);

                save |= Input.GetKeyDown(KeyCode.S) || (autoAdvance && next && index == ledCount - 1);

                revertToBackup |= Input.GetKeyDown(KeyCode.R);

                update |= autoAdvance || Input.GetKeyDown(KeyCode.Space);

                yield return null;
            }
        }
    }

    private bool IsBelowAgreementThreshold()
    {
        if (queuedGuesses.Count < queueSize)
        {
            return false;
        }

        float agreementValue = 0f;
        foreach (var guess in queuedGuesses)
        {
            agreementValue += (guess - averagePosition).magnitude;
        }
        agreementValue /= queueSize;

        return agreementValue < agreementThreshold;
    }

    private void UpdateAverage()
    {
        queuedGuesses.Enqueue(currentPosition);
        if (queuedGuesses.Count > queueSize) queuedGuesses.Dequeue();

        averagePosition = Vector2.zero;
        foreach (Vector2 pos in queuedGuesses)
        {
            averagePosition += pos;
        }
        averagePosition /= queuedGuesses.Count;
    }

    [ContextMenu(nameof(RefreshCurrentLED))]
    private IEnumerator RefreshCurrentLED()
    {
        if (currentLED != index)
        {
            DisableCurrentLED();
        }

        currentLED = index;
        dispatcher.UpdateLED(index, Color.green * colourStrength);

        yield return new WaitForSeconds(enableLedDelay);
    }

    private void DisableCurrentLED()
    {
        if (currentLED != -1)
        {
            dispatcher.UpdateLED(currentLED, Color.black);
            currentLED = -1;
        }
    }

    private void AdjustIndex(int offset)
    {
        index = Mathf.Clamp(index + offset, 0, ledCount - 1);
        print($"LED #{index}");
    }

    private Vector2 GetPositionFromTexture()
    {
        UpdateCachedTexture();

        Vector2Int maxPixelCoord = new Vector2Int();
        float maxPixelValue = -1;

        for (int x = 0; x < setupRT.width; x++)
        {
            for (int y = 0; y < setupRT.height; y++)
            {
                var pixelValue = CachedTexture.GetPixel(x, y).grayscale;
                if (pixelValue > maxPixelValue)
                {
                    maxPixelCoord = new Vector2Int(x, y);
                    maxPixelValue = pixelValue;
                }
            }
        }

        float normX = maxPixelCoord.x / (float)setupRT.width;
        float normY = maxPixelCoord.y / (float)setupRT.height;

        return new Vector2(normX, normY);
    }

    private void UpdateCachedTexture()
    {
        RenderTexture previousActive = RenderTexture.active;

        RenderTexture.active = setupRT;
        CachedTexture.ReadPixels(new Rect(0, 0, setupRT.width, setupRT.height), 0, 0);
        CachedTexture.Apply();

        RenderTexture.active = previousActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentPosition, currentGizmoSize);
        Gizmos.DrawWireSphere(currentPosition + Vector2.right, currentGizmoSize);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(averagePosition, averageGizmoSize);
        Gizmos.DrawWireSphere(averagePosition + Vector2.right, averageGizmoSize);


        if (index >= 0 && index < ledData.LightCount)
        {
            Vector2 storedPosition = ledData.GetPosition(index);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(storedPosition, storedGizmoSize);
            Gizmos.DrawWireSphere(storedPosition + Vector2.right, storedGizmoSize);
        }

        Gizmos.color = IsBelowAgreementThreshold() ? Color.green : Color.red;
        Gizmos.DrawCube(new Vector3(1f, .5f), new Vector3(0.01f, 1f, 0.01f));
    }
}

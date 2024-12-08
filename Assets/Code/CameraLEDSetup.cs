using System.Collections;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraLEDSetup : MonoBehaviour
{
    [SerializeField] private PositionProvider positionProvider;
    [SerializeField] private LEDDispatcher dispatcher;
    [Space]
    [SerializeField] private int ledStartIndex = 0;
    [SerializeField] private int ledCount = 100;
    [SerializeField] private LEDData ledData;
    [Space] 
    [SerializeField] private Color colour = Color.green;
    [SerializeField] private float enableLedDelay = 0.2f;
    [Space]
    [SerializeField, ReadOnly] private Vector2[] positions;
    [SerializeField, ReadOnly] private Vector2[] backupPositions;

    private int currentLED = -1;
    private int index = 0;
    private Vector2 currentPosition;
    private Vector2 averagePosition;

    private void OnEnable()
    {
        StartCoroutine(RunSetup());
    }

    private IEnumerator RunSetup()
    {
        yield return Initialise();

        while (gameObject)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                yield return AdjustIndexAsync(1);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                yield return AdjustIndexAsync(-1);
            }

            yield return null;
        }
        
        // while (gameObject)
        // {
        //     if (update)
        //     {
        //         positions[index] = averagePosition;
        //         update = false;
        //     }
        //
        //     if (save)
        //     {
        //
        //         save = false;
        //     }
        //
        //     if (next || previous)
        //     {
        //         
        //     }
        //
        //     currentPosition = positionProvider.GetPosition();
        //     UpdateTarget();
        //
        //     float delayStart = Time.time;
        //     while (Time.time - delayStart < updateDelay)
        //     {
        //         bool autoAdvance = !waitForUserInput && IsBelowAgreementThreshold();
        //         bool inputAdvance = Input.GetKeyDown(KeyCode.RightArrow);
        //         next |= autoAdvance || inputAdvance;
        //
        //         previous |= Input.GetKeyDown(KeyCode.LeftArrow);
        //
        //         update |= autoAdvance || Input.GetKeyDown(KeyCode.Space);
        //
        //         yield return null;
        //     }
        // }
    }

    private IEnumerator Initialise()
    {
        ledData.Backup();
        positions = new Vector2[ledCount];
        for (int i = 0; i < Mathf.Min(ledCount, ledData.LightCount); i++)
        {
            positions[i] = ledData.GetPosition(i);
        }

        index = ledStartIndex;
        yield return RefreshCurrentLED();
    }

    private IEnumerator AdjustIndexAsync(int offset)
    {
        index = Mathf.Clamp(index + offset, 0, ledCount - 1);
        Debug.Log($"LED #{index}");
        
        yield return RefreshCurrentLED();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void Save()
    {
        ledData.SetLEDPositions(positions);
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(ledData);
#endif
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void RevertToBackup()
    {
        ledData.LoadBackup();
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(ledData);
#endif
    }
    
    [Button]
    private IEnumerator RefreshCurrentLED()
    {
        if (currentLED != index)
        {
            DisableCurrentLED();
        }

        currentLED = index;
        dispatcher.UpdateLED(index, colour);

        yield return new WaitForSeconds(enableLedDelay);
        
        positionProvider.Setup(positions[currentLED]);
    }

    private void DisableCurrentLED()
    {
        if (currentLED != -1)
        {
            dispatcher.UpdateLED(currentLED, Color.black);
            currentLED = -1;
        }
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(currentPosition, currentGizmoSize);
    //     Gizmos.DrawWireSphere(currentPosition + Vector2.right, currentGizmoSize);
    //     
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(averagePosition, averageGizmoSize);
    //     Gizmos.DrawWireSphere(averagePosition + Vector2.right, averageGizmoSize);
    //
    //
    //     if (index >= 0 && index < ledData.LightCount)
    //     {
    //         Vector2 storedPosition = ledData.GetPosition(index);
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireSphere(storedPosition, storedGizmoSize);
    //         Gizmos.DrawWireSphere(storedPosition + Vector2.right, storedGizmoSize);
    //     }
    //
    //     Gizmos.color = IsBelowAgreementThreshold() ? Color.green : Color.red;
    //     Gizmos.DrawCube(new Vector3(1f, .5f), new Vector3(0.01f, 1f, 0.01f));
    // }
}
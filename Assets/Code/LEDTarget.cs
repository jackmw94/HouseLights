using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEDTarget : MonoBehaviour
{
    [SerializeField] private LEDArea targetArea;
    
    [SerializeField] private float sensitivity = 0.0001f;
    [SerializeField, ReadOnly] private bool overridden;

    private Vector2 guess;
    private Vector2 current;
    private Vector3? mousePreviousPosition;

    public bool IsOverridden => overridden;

    private void OnEnable()
    {
        Application.focusChanged += b =>
        {
            mousePreviousPosition = null;
        };
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            overridden = true;

            if (mousePreviousPosition.HasValue && !Input.GetMouseButtonDown(0))
            {
                current += (Vector2)(Input.mousePosition - mousePreviousPosition.Value) * sensitivity;
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            overridden = false;
        }

        if (!overridden)
        {
            current = guess;
        }

        transform.position = targetArea.GetWorldSpacePositionFromLEDPosition(current);

        mousePreviousPosition = Input.mousePosition;
    }

    public void SetTargetGuess(Vector2 guessedTarget)
    {
        guess = guessedTarget;
    }

    [Button]
    public void ResetTarget()
    {
        overridden = false;
    }
}
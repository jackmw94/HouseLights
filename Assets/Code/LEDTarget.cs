using NaughtyAttributes;
using UnityEngine;

public class LEDTarget : PositionProvider
{
    [SerializeField] private LEDArea targetArea;
    
    [SerializeField] private float sensitivity = 0.0001f;
    [SerializeField, ReadOnly] private bool overridden;

    private Vector2 guess;
    private Vector2 current;
    private Vector3? mousePreviousPosition;

    private void OnEnable()
    {
        Application.focusChanged += b =>
        {
            mousePreviousPosition = null;
        };
    }

    public override void Setup(Vector2? initialPosition = null)
    {
        overridden = false;
    }

    public override Vector2 GetPosition()
    {
        return current;
    }

    public override bool IsConfident()
    {
        return overridden;
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
}
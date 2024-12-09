using UnityEngine;

public class CompositePositionProvider : PositionProvider
{
    [SerializeField] private LEDTarget ledTarget;
    [SerializeField] private PositionProvider automaticPositionProvider;
    [Space] 
    [SerializeField] private bool autoAdvance;

    public override void Setup(Vector2? initialPosition = null)
    {
        ledTarget.Setup(initialPosition);
        
        automaticPositionProvider.enabled = true;
        automaticPositionProvider.Setup(initialPosition);
    }

    public override Vector2 GetPosition()
    {
        if (ledTarget.IsConfident())
        {
            Vector2 inputPosition = ledTarget.GetPosition();
            return inputPosition;
        }

        Vector2 guessedPosition = automaticPositionProvider.GetPosition();
        return guessedPosition;
    }

    public override bool IsConfident()
    {
        if (ledTarget.IsConfident())
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        if (automaticPositionProvider.IsConfident())
        {
            return autoAdvance;
        }

        return false;
    }

    private void Update()
    {
        if (ledTarget.IsConfident())
        {
            automaticPositionProvider.enabled = false;
            return;
        }
        
        Vector2 guessedTarget = automaticPositionProvider.GetPosition();
        ledTarget.SetTargetGuess(guessedTarget);
    }
}
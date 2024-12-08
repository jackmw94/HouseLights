using UnityEngine;

public class CompositePositionProvider : PositionProvider
{
    [SerializeField] private LEDTarget ledTarget;
    [SerializeField] private PositionProvider automaticPositionProvider;

    public override void Setup(Vector2? initialPosition = null)
    {
        ledTarget.Setup(initialPosition);
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
        return ledTarget.IsConfident() || automaticPositionProvider.IsConfident();
    }

    private void Update()
    {
        Vector2 guessedTarget = automaticPositionProvider.GetPosition();
        ledTarget.SetTargetGuess(guessedTarget);
    }
}
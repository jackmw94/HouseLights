using UnityEngine;

public abstract class PositionProvider : MonoBehaviour
{
    public abstract void Setup(Vector2? initialPosition = null);
    public abstract Vector2 GetPosition();
    public abstract bool IsConfident();
}
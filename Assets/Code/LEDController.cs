using UnityEngine;

public class LEDController : MonoBehaviour
{
    [SerializeField] private LEDData lightData;
    [SerializeField] private Vector2 tiling = Vector2.one;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float rotation = 0f;
    [SerializeField] private int limitLEDCount = -1;
    [SerializeField] private Rect validLEDBounds;
    [Space]
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private LEDDispatcher dispatcher;
    [Space]
    [SerializeField] private bool gizmosUpdateColourInPlayMode = false;
    [SerializeField] private float gizmoSize = 0.01f;

    private Color[] colours;

    Texture2D cachedTexture_BackingField;
    Texture2D CachedTexture => cachedTexture_BackingField ? cachedTexture_BackingField : (cachedTexture_BackingField = new Texture2D(renderTexture.width, renderTexture.height));

    private bool ShouldGizmosUpdateColour => gizmosUpdateColourInPlayMode || !Application.isPlaying;
    private int LEDCount => limitLEDCount == -1 ? lightData.LightCount : Mathf.Min(limitLEDCount, lightData.LightCount);

    private void Awake()
    {
        ResetAll();
    }

    private void OnDestroy()
    {
        ResetAll();
    }

    private void ResetAll()
    {
        colours = new Color[lightData.LightCount];
        for (int i = 0; i < lightData.LightCount; i++)
        {
            colours[i] = Color.black;
            dispatcher.UpdateLED(i, colours[i]);
        }
    }

    private void Update()
    {
        UpdateCachedTexture();

        for (int i = 0; i < LEDCount; i++)
        {
            Vector2 ledPosition = GetPosition(i);
            if (!validLEDBounds.Contains(ledPosition))
            {
                continue;
            }

            Color current = GetCurrentColour(ledPosition);
            Color previous = colours[i];

            if (!ColoursEqual_NoAlpha(current, previous))
            {
                dispatcher.UpdateLED(i, current);
                colours[i] = current;
            }
        }
    }

    private bool ColoursEqual_NoAlpha(Color c1, Color c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }

    [ContextMenu(nameof(DebugUpdate))]
    private void DebugUpdate()
    {
        dispatcher.UpdateLED(0, Color.magenta);
    }

    private Color GetCurrentColour(Vector2 normalisedPosition)
    {
        int textureX = Mathf.FloorToInt(normalisedPosition.x * CachedTexture.width);
        int textureY = Mathf.FloorToInt(normalisedPosition.y * CachedTexture.height);
        Color pixel = CachedTexture.GetPixel(textureX, textureY);
        return pixel;
    }

    private void UpdateCachedTexture()
    {
        RenderTexture previousActive = RenderTexture.active;

        RenderTexture.active = renderTexture;
        CachedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        CachedTexture.Apply();

        RenderTexture.active = previousActive;
    }

    private void OnDrawGizmos()
    {
        if (ShouldGizmosUpdateColour)
        {
            UpdateCachedTexture();
        }

        Gizmos.DrawWireCube(validLEDBounds.center, validLEDBounds.size);

        for (int i = 0; i < LEDCount; i++)
        {
            Vector2 pos = GetPosition(i);
            Color pixel = ShouldGizmosUpdateColour ? GetCurrentColour(pos) : colours[i];
            Color inverted = new Color(1f - pixel.r, 1f - pixel.g, 1f - pixel.b);

            bool isValid = validLEDBounds.Contains(pos);
            inverted.a = isValid ? 1f : 0.2f;

            Gizmos.color = inverted;
            Gizmos.DrawSphere(new Vector3(pos.x, pos.y, 0f), gizmoSize);
        }
    }

    private Vector2 GetPosition(int ledId)
    {
        Vector2 midWay = Vector2.one / 2f;

        Vector2 transformedPosition = lightData.GetPosition(ledId);

        transformedPosition = transformedPosition + offset;
        transformedPosition = (transformedPosition - midWay) * tiling + midWay;
        transformedPosition = (Vector2)(Quaternion.AngleAxis(rotation, Vector3.forward) * (transformedPosition - midWay)) + midWay;

        transformedPosition.x = Mathf.Clamp01(transformedPosition.x);
        transformedPosition.y = Mathf.Clamp01(transformedPosition.y);
        return transformedPosition;
    }
}

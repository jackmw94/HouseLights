using UnityEngine;

public class ImageDetectionPositionProvider : PositionProvider
{
    [SerializeField] private RenderTexture setupRT;
    
    Texture2D cachedTexture;
    Texture2D CachedTexture => cachedTexture ? cachedTexture : (cachedTexture = new Texture2D(setupRT.width, setupRT.height));
    
    public override Vector2 GetPosition()
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
}
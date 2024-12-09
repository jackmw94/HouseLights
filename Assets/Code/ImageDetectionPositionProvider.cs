using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ImageDetectionPositionProvider : PositionProvider
{
    [SerializeField] private RenderTexture setupRT;
    [Space] 
    [SerializeField, Min(1)] private int updateEveryNFrames = 10;
    [SerializeField] private int queueSize = 20;
    [SerializeField] private int kernelSize = 7;
    [SerializeField] private int stride = 3;
    [SerializeField] private bool flipY;
    [Space]
    [SerializeField] private float agreementThreshold = 0.0075f;
    
    private readonly Queue<Vector2> queuedGuesses = new();
    
    Texture2D cachedTexture;
    Texture2D CachedTexture => cachedTexture ? cachedTexture : (cachedTexture = new Texture2D(setupRT.width, setupRT.height));
    
    private void Update()
    {
        if (Time.frameCount % updateEveryNFrames == 0)
        {
            UpdateGuesses();
        }
    }

    public override void Setup(Vector2? initialPosition = null)
    {
        queuedGuesses.Clear();
    }

    public override Vector2 GetPosition()
    {
        return GetAveragePosition();
    }

    public override bool IsConfident()
    {
        float agreementValue = GetAgreementValue();
        return agreementValue < agreementThreshold;
    }

    private void UpdateGuesses()
    {
        Vector2 nextGuess = GetGuess();
        
        queuedGuesses.Enqueue(nextGuess);
        if (queuedGuesses.Count > queueSize)
        {
            queuedGuesses.Dequeue();
        }
    }

    private Vector2 GetGuess()
    {
        UpdateCachedTexture();
        
        Vector2Int maxPixelCoordinate = new();
        float maxValue = float.MinValue;

        foreach (ConvolutionUtility.ConvolutionContext convolutionContext in ConvolutionUtility.Convolve(CachedTexture, kernelSize, stride))
        {
            float kernelValue = convolutionContext.Pixels.Sum(p => p.grayscale);
            if (kernelValue > maxValue)
            {
                maxPixelCoordinate = convolutionContext.Centre;
                maxValue = kernelValue;
            }
        }
        
        float normX = maxPixelCoordinate.x / (float)setupRT.width;
        float normY = maxPixelCoordinate.y / (float)setupRT.height;

        if (flipY)
        {
            normY = 1f - normY;
        }
        
        return new Vector2(normX, normY);
    }

    private Vector2 GetAveragePosition()
    {
        if (queuedGuesses.Count == 0)
        {
            return Vector2.zero;
        }
        
        Vector2 averagePosition = Vector2.zero;
        foreach (Vector2 queuedGuess in queuedGuesses)
        {
            averagePosition += queuedGuess;
        }
        
        averagePosition /= queuedGuesses.Count;
        return averagePosition;
    }

    private void UpdateCachedTexture()
    {
        RenderTexture previousActive = RenderTexture.active;

        RenderTexture.active = setupRT;
        CachedTexture.ReadPixels(new Rect(0, 0, setupRT.width, setupRT.height), 0, 0);
        CachedTexture.Apply();

        RenderTexture.active = previousActive;
    }

    private IEnumerable<Vector2Int> GetPixelCoordinates()
    {
        for (int x = 0; x < setupRT.width; x++)
        {
            for (int y = 0; y < setupRT.height; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    private float GetAgreementValue()
    {
        if (queuedGuesses.Count < queueSize)
        {
            return 0f;
        }

        float agreementValue = 0f;
        foreach (Vector2 guess in queuedGuesses)
        {
            agreementValue += (guess - GetAveragePosition()).magnitude;
        }
        agreementValue /= queueSize;

        return agreementValue;
    }

    [Button]
    private void ResetGuesses()
    {
        queuedGuesses.Clear();
    }
}
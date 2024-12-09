using System;
using System.Collections.Generic;
using UnityEngine;

public static class ConvolutionUtility
{
    public struct ConvolutionContext
    {
        public Color[] Pixels;
        public Vector2Int Centre;
    }

    public static IEnumerable<ConvolutionContext> Convolve(Texture2D texture, int kernelSize = 3, int stride = 1)
    {
        for (int x = 0; x < texture.width - kernelSize; x += stride)
        {
            for (int y = 0; y< texture.height - kernelSize; y += stride)
            {
                yield return new ConvolutionContext
                {
                    Pixels = texture.GetPixels(x, y, kernelSize, kernelSize),
                    Centre = new Vector2Int(x + (kernelSize - 1) / 2, y + (kernelSize - 1) / 2)
                };
            }
        }
    }
}
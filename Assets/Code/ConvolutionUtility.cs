using System;
using UnityEngine;

public static class ConvolutionUtility
{
    public struct ConvolutionContext
    {
        public Color[] Pixels;
        public int CentreX;
        public int CentreY;
    }

    public static void Convolve(Texture2D texture, Action<ConvolutionContext> onConvolution, int kernelSize = 3, int stride = 1)
    {
        for (int x = 0; x < texture.width - kernelSize; x += stride)
        {
            for (int y = 0; y< texture.height - kernelSize; y += stride)
            {
                onConvolution(new ConvolutionContext
                {
                    Pixels = texture.GetPixels(x, y, kernelSize, kernelSize),
                    CentreX = x + (kernelSize - 1) / 2,
                    CentreY = y + (kernelSize - 1) / 2,
                });
            }
        }
    }
}
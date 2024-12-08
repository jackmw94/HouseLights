using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayWebCam : MonoBehaviour
{
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        // for debugging purposes, prints available devices to the console
        for (int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        // assuming the first available WebCam is desired
        WebCamTexture tex = new WebCamTexture(devices[0].name);
        foreach (Renderer renderer in renderers)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetTexture(BaseMap, tex);
            renderer.SetPropertyBlock(propertyBlock);
        }
        tex.Play();
    }
}

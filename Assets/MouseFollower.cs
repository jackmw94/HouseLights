using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxScale = new Vector2(0.01f, 2f);
    [SerializeField] private float scaleAdjustmentSpeed = 0.1f;

    private float scale;

    private void Awake()
    {
        scale = transform.localScale.x;
    }

    private void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0f);

        scale += Input.mouseScrollDelta.y * Time.deltaTime * scaleAdjustmentSpeed;
        scale = Mathf.Clamp(scale, 0.01f, 5f);
        transform.localScale = scale * Vector3.one;
    }
}

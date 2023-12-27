using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongPaddle : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float limit;
    [Space]
    [SerializeField] private KeyCode upKey;
    [SerializeField] private KeyCode downKey;

    private void Update()
    {
        bool moveUp = Input.GetKey(upKey);
        bool moveDown = Input.GetKey(downKey);

        float movement = (moveUp ? speed : 0f) + (moveDown ? -speed : 0f);
        movement *= Time.deltaTime;

        Vector3 nextPosition = transform.localPosition + Vector3.up * movement;
        nextPosition.y = Mathf.Clamp(nextPosition.y, -limit, limit);
        transform.localPosition = nextPosition;

    }
}

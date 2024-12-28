using UnityEngine;

public class MonitorMover : MonoBehaviour
{
    [SerializeField] private float keyboardMovementSpeed = -5f;
    [SerializeField] private float mouseMovementSpeed = -1f;
    [SerializeField] private float zoomSpeed = -15f;

    private Vector2 previousMousePosition = Vector2.zero;
    
    private void Update()
    {
        Vector2 movement = GetKeyboardMovement() + GetMouseMovement();

        transform.position += new Vector3(movement.x, movement.y, 0f);
        transform.localScale = GetZoomScale() * Vector3.one;
        
        previousMousePosition = Input.mousePosition;
    }

    private float GetZoomScale()
    {
        float zoom = IsMouseWithinScreen() ? Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed : 0f;
        float localScale = transform.localScale.x * (1f + zoom);
        localScale = Mathf.Clamp(localScale, 0.01f, 20f);
        
        if (float.IsNaN(localScale)) localScale = transform.localScale.x;

        return localScale;
    }

    private bool IsMouseWithinScreen()
    {
        Vector3 mousePosition = Input.mousePosition;
        return mousePosition.x >= 0 && mousePosition.x < Screen.width && mousePosition.y >= 0 && mousePosition.y < Screen.height;
    }

    private Vector2 GetMouseMovement()
    {
        Vector2 movement = Vector2.zero;
        if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
        { 
            movement = (Vector2)Input.mousePosition - previousMousePosition;
            movement *= (Time.deltaTime * mouseMovementSpeed);
        }
        
        return movement;
    }

    private Vector2 GetKeyboardMovement()
    {
        Vector2 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) movement += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) movement += Vector2.right;
        if (Input.GetKey(KeyCode.UpArrow)) movement += Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) movement += Vector2.down;
        movement *= (Time.deltaTime * keyboardMovementSpeed);
        
        return movement;
    }
}
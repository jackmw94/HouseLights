using System;
using UnityEngine;

public class LEDArea : MonoBehaviour
{
    public Vector3 GetWorldSpacePositionFromLEDPosition(Vector2 ledPosition)
    {
        return transform.TransformPoint(ledPosition - Vector2.one / 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetWorldSpacePositionFromLEDPosition(Vector2.zero), 0.025f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetWorldSpacePositionFromLEDPosition(Vector2.one), 0.025f);
    }
}
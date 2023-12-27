using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    [SerializeField] private Transform sides;
    [SerializeField] private Transform endZone;
    [SerializeField] private GameObject winnerDisplay;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float startVelocity = 7;
    [SerializeField] private float maxVelocity = 30;
    [SerializeField] private int hitsToMaxVelocity = 14;
    [SerializeField] private float verticalLimit;

    private Vector3 startPos;
    private int hitCount = 0;

    private void OnEnable()
    {
        startPos = transform.position;
        StartCoroutine(Restart(1f));
    }

    private IEnumerator Restart(float movementDelay)
    {
        hitCount = 0;

        rb.position = startPos;
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(movementDelay);

        rb.velocity = (new Vector3(1f, 1f, 0f) * startVelocity);
    }

    private IEnumerator Winner(float restartDelay)
    {
        rb.position = startPos;
        rb.velocity = Vector3.zero;

        winnerDisplay.SetActive(true);
        yield return new WaitForSeconds(restartDelay);
        winnerDisplay.SetActive(false);

        yield return Restart(1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent == endZone)
        {
            StartCoroutine(Winner(2f));
            return;
        }

        if (other.gameObject.transform.parent == sides)
        {
            bool isGoingUp = Vector3.Dot(rb.velocity, Vector3.up) > 0;
            Vector3 sideReflectionVector = isGoingUp ? Vector3.down : Vector3.up;
            rb.velocity = Vector3.Reflect(rb.velocity, sideReflectionVector);
            return;
        }

        hitCount++;
        float velocityLerp = Mathf.Clamp01(hitCount / (float)hitsToMaxVelocity);
        float currentVelocity = Mathf.Lerp(startVelocity, maxVelocity, velocityLerp);

        bool isGoingRight = Vector3.Dot(rb.velocity, Vector3.right) > 0;
        Vector3 paddleReflectionVector = isGoingRight ? Vector3.left : Vector3.right;
        rb.velocity = Vector3.Reflect(rb.velocity, paddleReflectionVector);
        //rb.velocity = (collision.contacts[0].normal * currentVelocity * 2f) * Time.fixedDeltaTime;
    }
}

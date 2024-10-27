using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 resetPosition = new Vector3(0, 1, 0);
    public float stopThreshold = 0.1f;
    public float stationaryTimeThreshold = 1f;
    public float resetDelay = 1.0f;
    private Rigidbody rb;
    private bool isResetting = false;
    private float timeStationary = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < stopThreshold)
        {
            timeStationary += Time.deltaTime;
            if (timeStationary >= stationaryTimeThreshold && !isResetting)
            {
                isResetting = true;
                Invoke("ResetBallPosition", resetDelay);
            }
        } else {
            timeStationary = 0f;
        }
    }

    private void ResetBallPosition()
    {
        transform.position = resetPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isResetting = false;
        timeStationary = 0f;
    }
}

using UnityEngine;

public class BallStuckHandler : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("Upward force applied when jumping.")]
    public float upwardForce = 100f;

    [Tooltip("Time between automatic jumps (in seconds).")]
    public float jumpInterval = 3f;  

    [Tooltip("Time to disable gravity during launch.")]
    public float gravityDisableTime = 0.5f;

    [Tooltip("Only jump if velocity is below this threshold (stuck detection).")]
    public float stuckVelocityThreshold = 0.5f; 

    private Rigidbody rb;
    private bool gravityWasEnabled;
    private float jumpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("BallStuckHandler requires a Rigidbody component.");
            enabled = false;
            return;
        }

        Debug.Log($"BallStuckHandler AUTO mode - Jumps every {jumpInterval}s when stuck");
    }

    void Update()
    {
       
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpInterval && IsBallStuck())
        {
            Jump();
            jumpTimer = 0f;  // Reset timer
        }
    }

    bool IsBallStuck()
    {
        // Ball is "stuck" if moving too slowly
        return rb.linearVelocity.magnitude < stuckVelocityThreshold;
    }

    public void Jump()
    {
        Debug.Log($"AUTO Jump! Velocity: {rb.linearVelocity.magnitude:F2}");

        // Disable gravity temporarily
        gravityWasEnabled = rb.useGravity;
        rb.useGravity = false;

        // Apply upward force + random for natural feel
        Vector3 jumpForce = Vector3.up * upwardForce + Random.insideUnitSphere * 10f;
        rb.AddForce(jumpForce, ForceMode.Impulse);

        // Ensure minimum upward velocity
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, upwardForce / rb.mass * 0.8f),
            rb.linearVelocity.z
        );

        // Re-enable gravity after delay
        Invoke(nameof(ReEnableGravity), gravityDisableTime);
    }

    private void ReEnableGravity()
    {
        rb.useGravity = gravityWasEnabled;
    }

   
    public void JumpButton()
    {
        Jump();
        jumpTimer = 0f;  // Reset auto-timer
    }
}
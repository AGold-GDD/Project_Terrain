using UnityEngine;

public class BallStuckHandler : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("Upward force applied when jumping.")]
    public float upwardForce = 100f;

    [Tooltip("Cooldown time after jumping (in seconds).")]
    public float jumpCooldown = 1f;

    [Tooltip("Time to disable gravity during launch.")]
    public float gravityDisableTime = 0.5f;

    [Tooltip("Minimum velocity magnitude required to jump.")]
    public float movementThreshold = 0.1f;

    [Header("Input Settings")]
    [Tooltip("Key to trigger jump (default: Space).")]
    public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody rb;
    private bool canJump = true;
    private bool gravityWasEnabled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("BallStuckHandler requires a Rigidbody component on the GameObject.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // FIXED: Check input BEFORE character controller processes it
        if (Input.GetKeyDown(KeyCode.Space) && canJump && rb.linearVelocity.magnitude > movementThreshold)
        {
            Jump();
            return; // Prevent other scripts from using Space
        }
    }

    public void Jump()
    {
        Debug.Log($"Player triggered jump! Velocity before: {rb.linearVelocity}, Mass: {rb.mass}");

        // Temporarily disable gravity to prevent immediate fall
        gravityWasEnabled = rb.useGravity;
        rb.useGravity = false;

        // Apply upward force + slight random for natural feel
        rb.AddForce(Vector3.up * upwardForce + Random.insideUnitSphere * 10f, ForceMode.Impulse);

        // Ensure minimum upward velocity
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, upwardForce / rb.mass * 0.8f),
            rb.linearVelocity.z
        );

        // Start cooldown
        canJump = false;
        Invoke(nameof(ResetCooldown), jumpCooldown);

        // Re-enable gravity after delay
        Invoke(nameof(ReEnableGravity), gravityDisableTime);

        Debug.Log($"Jump executed! Velocity after: {rb.linearVelocity}");
    }

    private void ReEnableGravity()
    {
        rb.useGravity = gravityWasEnabled;
    }

    private void ResetCooldown()
    {
        canJump = true;
    }


    public void JumpButton()
    {
        Jump();
    }
}
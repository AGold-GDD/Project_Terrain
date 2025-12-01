using UnityEngine;

public class BallStuckHandler : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds before triggering the upward shoot.")]
    public float stuckTimeThreshold = 30f;

    [Tooltip("Minimum velocity magnitude to reset the timer.")]
    public float movementThreshold = 0.1f;

    [Tooltip("Upward force applied when stuck.")]
    public float upwardForce = 100f;  // Increased significantly

    [Tooltip("Cooldown time after shooting to prevent spam.")]
    public float shootCooldown = 1f;

    [Tooltip("Time to disable gravity during launch.")]
    public float gravityDisableTime = 0.5f;

    private Rigidbody rb;
    private float stuckTimer = 0f;
    private bool canShoot = true;
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

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > movementThreshold)
        {
            stuckTimer = 0f;
        }
        else
        {
            stuckTimer += Time.fixedDeltaTime;

            if (stuckTimer >= stuckTimeThreshold && canShoot)
            {
                ShootUp();
                stuckTimer = 0f;
                canShoot = false;
                Invoke(nameof(ResetCooldown), shootCooldown);
            }
        }
    }

    private void ShootUp()
    {
        // Log before
        Debug.Log($"Launching ball. Velocity before: {rb.linearVelocity}, Mass: {rb.mass}");

        // Temporarily disable gravity to prevent immediate fall
        gravityWasEnabled = rb.useGravity;
        rb.useGravity = false;

        // Apply a massive upward force + velocity override for reliability
        rb.AddForce(Vector3.up * upwardForce + Random.insideUnitSphere * 10f, ForceMode.Impulse);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, upwardForce / rb.mass), rb.linearVelocity.z);  // Ensure upward velocity

        // Re-enable gravity after a short delay
        Invoke(nameof(ReEnableGravity), gravityDisableTime);

        Debug.Log($"Ball launched! Velocity after: {rb.linearVelocity}");
    }

    private void ReEnableGravity()
    {
        rb.useGravity = gravityWasEnabled;
    }

    private void ResetCooldown()
    {
        canShoot = true;
    }
}
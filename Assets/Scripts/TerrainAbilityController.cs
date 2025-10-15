using UnityEngine;

public class TerrainAbilityController : MonoBehaviour
{
    [Header("Ability Settings")]
    public float maxAbilityAmount = 100f;  // Total amount of ability available
    public float rechargeRate = 10f;       // Amount recharged per second

    [Header("Regeneration Delay")]
    public float regenerationDelay = 8f;   // Seconds to wait before starting recharge after depletion

    private float currentAbilityAmount;
    private float lastDepletedTime = -1f;  // Time when ability last hit zero
    private bool isDepleted = false;       // Flag to track depleted state

    void Start()
    {
        currentAbilityAmount = maxAbilityAmount;  // Start fully charged
        isDepleted = false;
    }

    void Update()
    {
        RechargeAbility();
    }

    // Call this method when the player uses the ability
    public bool UseAbility(float amount)
    {
        if (currentAbilityAmount >= amount)
        {
            currentAbilityAmount -= amount;

            // Check if we've just depleted the ability
            if (currentAbilityAmount <= 0f)
            {
                currentAbilityAmount = 0f;  // Clamp to zero
                isDepleted = true;
                lastDepletedTime = Time.time;
                Debug.Log("Ability depleted! Regeneration starts in " + regenerationDelay + " seconds.");
            }

            return true;  // Ability used successfully
        }
        else
        {
            return false; // Not enough ability left
        }
    }

    private void RechargeAbility()
    {
        // If depleted, wait for the delay before starting recharge
        if (isDepleted && (Time.time - lastDepletedTime) < regenerationDelay)
        {
            return;  // Don't recharge yet
        }

        // Start recharging if delay has passed and we're below max
        if (currentAbilityAmount < maxAbilityAmount)
        {
            if (isDepleted)
            {
                isDepleted = false;  // Reset depleted state
                Debug.Log("Regeneration starting now!");
            }

            currentAbilityAmount += rechargeRate * Time.deltaTime;
            currentAbilityAmount = Mathf.Min(currentAbilityAmount, maxAbilityAmount);
        }
    }

    // Optional: expose current ability amount for UI
    public float GetCurrentAbilityAmount()
    {
        return currentAbilityAmount;
    }

    // Optional: Check if currently in delay period
    public bool IsInRegenerationDelay()
    {
        return isDepleted;
    }
}

using UnityEngine;

public class TerrainAbilityController : MonoBehaviour
{
    [Header("Ability Settings")]
    public float maxAbilityAmount = 100f;  // Total amount of ability available
    public float rechargeRate = 15f;       // Amount recharged per second

    private float currentAbilityAmount;

    void Start()
    {
        currentAbilityAmount = maxAbilityAmount;  // Start fully charged
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
            return true;  // Ability used successfully
        }
        else
        {
            return false; // Not enough ability left
        }
    }

    private void RechargeAbility()
    {
        if (currentAbilityAmount < maxAbilityAmount)
        {
            currentAbilityAmount += rechargeRate * Time.deltaTime;
            currentAbilityAmount = Mathf.Min(currentAbilityAmount, maxAbilityAmount);
        }
    }

    // Optional: expose current ability amount for UI
    public float GetCurrentAbilityAmount()
    {
        return currentAbilityAmount;
    }
}

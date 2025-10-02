using UnityEngine;
using UnityEngine.UI;

public class AbilityBarUI : MonoBehaviour
{
    public TerrainAbilityController abilityController;
    public Slider abilitySlider;

    void Start()
    {
        abilitySlider.maxValue = abilityController.maxAbilityAmount;
        abilitySlider.value = abilityController.GetCurrentAbilityAmount();
    }

    void Update()
    {
        abilitySlider.value = abilityController.GetCurrentAbilityAmount();
    }
}

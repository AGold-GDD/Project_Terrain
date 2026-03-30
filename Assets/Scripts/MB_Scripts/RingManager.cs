using UnityEngine;
using TMPro;

public class RingManager : MonoBehaviour
{
    [Header("Setup")]
    public RingTrigger[] rings;
    public TextMeshPro worldText;
    public GameObject goalBlock;

    private int ringsPassed = 0;

    void Start()
    {
        UpdateText();
        if (goalBlock) goalBlock.SetActive(true);
    }


    public void OnRingPassed()
    {
        ringsPassed++;
        UpdateText();

        if (ringsPassed >= rings.Length)
        {
            if (goalBlock) goalBlock.SetActive(false);
        }
    }

    void UpdateText()
    {
        if (worldText)
            worldText.text = $"Rings: {ringsPassed}/{rings.Length}";
    }
}
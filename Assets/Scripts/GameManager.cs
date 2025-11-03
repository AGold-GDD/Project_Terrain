using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int collectedCount = 0;
    public TextMeshProUGUI collectibleCounterText;  // Assign in Inspector

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectSphere()
    {
        collectedCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        collectibleCounterText.text = "Spheres Collected: " + collectedCount;
    }
}

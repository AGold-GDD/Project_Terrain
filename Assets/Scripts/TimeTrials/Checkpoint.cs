using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Time Bonus")]
    public float timeBonus = 10f; // How many seconds to add

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Save position
            CheckpointManager.Instance.SetCheckpoint(transform.position);

            // Add Time
            GameManager.Instance.AddTime(timeBonus);

            // Optional: Visual feedback
             GetComponent<Renderer>().material.color = Color.green;
        }
    }
}
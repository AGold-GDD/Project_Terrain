using UnityEngine;

public class CollectibleSphere : MonoBehaviour
{
    // This method is called when any trigger collider attached to this GameObject or its children is entered
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify GameManager or Player about collection
            GameManager.instance.CollectSphere();

            // Destroy the collectible sphere
            Destroy(gameObject);
        }
    }
}

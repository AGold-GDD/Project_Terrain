using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    public RingManager ringManager; // Drag RingManager here!

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && ringManager != null)
        {
            gameObject.SetActive(false); // Disappear
            ringManager.OnRingPassed(); // Tell manager!
        }
    }
}
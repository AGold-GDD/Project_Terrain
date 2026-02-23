using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    public TrackManager trackManager;
    public int thisCheckpointIndex;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cart"))
        {
            Debug.Log("Checkpoint reached");
            trackManager.PassedCheckPoint(thisCheckpointIndex);
        }
    }
}

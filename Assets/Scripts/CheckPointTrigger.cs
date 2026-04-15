using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    public TrackManager trackManager;
    public int thisCheckpointIndex;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Checkpoint reached");
            trackManager.PassedCheckPoint(thisCheckpointIndex);
        }
    }
}

using UnityEngine;

public class TimeTrialEnd : MonoBehaviour
{
    public GameManager GameManager;
    public float TimeForLevel;
    public GameObject TimeTrial;
    public float TimeSetLevel;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            TimeForLevel = GameManager.currentTime;
            Debug.Log(TimeForLevel);
            GameManager.currentTime = TimeSetLevel;
            TimeTrial.SetActive(false);

        }

    }
}

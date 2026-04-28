using UnityEngine;

public class TimeTrialEnd : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject TimeTrial;
    public PlayerData PlayerData;

    public int PaintLevel;
    public float TimeForLevel;
    public float TimeSetLevel;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            TimeForLevel = GameManager.currentTime;
            Debug.Log(TimeForLevel);
            GameManager.currentTime = TimeSetLevel;
            TimeTrial.SetActive(false);
            switch (PaintLevel)
            {
                case 1:
                    PlayerData.PaintLevel1 = TimeSetLevel;
                    break;
                case 2:
                    PlayerData.PaintLevel2 = TimeSetLevel;
                    break;
                case 3:
                    PlayerData.PaintLevel3 = TimeSetLevel;
                    break;      
            }
        }

    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlopeMinigame : MonoBehaviour
{

    public TextMeshProUGUI Timer;

    private float TimeSlope = 0;

    public bool TimerCheck = false;

    public void Update()
    {
        if (TimerCheck)
        {
            TimeStart();
        }
        else if (!TimerCheck)
        {
            TimerCheck = false;
        }


        Timer.text = $"{TimeSlope:F2}";
    }

    public void TimeStart()
    {
        Debug.Log("Timer is starting");
        TimeSlope += Time.deltaTime; 
    }

    public void TimeStop()
    {
        Debug.Log("Timer is stopping");
        TimeSlope += 0;
    }


}

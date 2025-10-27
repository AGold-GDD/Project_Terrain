/*using UnityEditor.UI;*/
using UnityEngine;

public class StartingSlope : MonoBehaviour
{
    public SlopeMinigame slopeMinigame;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cart")
        {
            Debug.Log("Starting");
            slopeMinigame.Timer.SetActive(true);
            slopeMinigame.TimerCheck = true;
            //this.gameObject.SetActive(false);
        }
    }
}

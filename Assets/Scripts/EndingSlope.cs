using UnityEngine;

public class EndingSlope : MonoBehaviour
{
    public SlopeMinigame slopeMinigame;

    private int SampleAmount = 0;
    private int AmountNeeded = 5;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cart")
        {
            Debug.Log("Ending");
            slopeMinigame.TimerCheck = false;
            //this.gameObject.SetActive(false);
        } 
        else if(other.gameObject.tag == "Samples")
        {
            Debug.Log("Sample collected");
            ++SampleAmount;
        }
    }
}

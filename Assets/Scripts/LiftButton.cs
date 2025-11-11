using UnityEngine;

public class LiftButton : MonoBehaviour
{
    public TheLift theLift;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            theLift.LiftIsOn = true;
        }
    }

    public void OnTriggerExit(Collider other) { 
        if (other.gameObject.CompareTag("Player"))
        {
            theLift.LiftIsOn = false;
        }
    }
}

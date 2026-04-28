using UnityEngine;

public class TimeTrialStart : MonoBehaviour
{

    public GameObject Manager;

    public void Start()
    {
        Manager.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Manager.SetActive(true);
        }
    }
}

using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == ("Player"))
        {
            Debug.Log("Checkpoint reached");
        }
    }
}

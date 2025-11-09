using UnityEngine;

public class DeadlyWater : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("piosin");
        }
    }
}

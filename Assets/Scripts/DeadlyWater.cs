using UnityEngine;

public class DeadlyWater : MonoBehaviour
{
    public GameObject playere;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playere)
        {
            Debug.Log("piosin");
        }
    }
}

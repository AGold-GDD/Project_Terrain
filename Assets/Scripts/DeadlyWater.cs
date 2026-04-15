using UnityEngine;

public class DeadlyWater : MonoBehaviour
{
    public NO_JETPACK_SimpleCharacterController player;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("piosin");
            player.PlayerRespawn();
        }
    }
}

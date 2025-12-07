using UnityEngine;

public class DeadlyWater : MonoBehaviour
{
    //public GameObject playerRespawn;

    public Transform Player;

    public CartMinigame cartMinigame;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("piosin");
            Player.position = cartMinigame.currentResPoint.position;
        }
    }
}

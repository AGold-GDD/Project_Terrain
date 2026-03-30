using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToSlide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Teleport slide");
            SceneManager.LoadScene("ParkourMinigame");
        }
    }
}

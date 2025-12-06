using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToOpenWorld : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Teleport slide");
            SceneManager.LoadScene("OpenWorld");
        }
    }
}
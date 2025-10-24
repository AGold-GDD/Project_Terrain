using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportToMonkeyBall : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("going to the Monkey ball minigame");
        SceneManager.LoadScene("");
    }
}

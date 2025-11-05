using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "NextLevel";  // Name of the scene to load (must be in Build Settings)
    [SerializeField] private string playerTag = "Player";  // Tag of the player GameObject (default: "Player")

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player entered trigger area. Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
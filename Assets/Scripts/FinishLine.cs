using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public GameObject MCPanel;

    public void Start()
    {
       MCPanel.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MCPanel.SetActive(true);
        }
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("NewMainLobby");
    }
}


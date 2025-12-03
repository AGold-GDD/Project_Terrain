using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    private bool IsPaused;

    private void Start()
    {
        IsPaused = false;
    }
    void Update()
    {
        PausePanel.SetActive(IsPaused);

        if (Input.GetKeyUp(KeyCode.Escape) && !IsPaused)
        {
            Paused();
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && IsPaused)
        {
            Resume();
        }
    }

    public void Paused()
    {
        IsPaused = true;
        //Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        IsPaused = false;
        //Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainLobby");
    }
}

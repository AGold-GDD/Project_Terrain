using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    private bool IsPaused;

    public GameObject TerrainUI;
    public GameObject GravityUI;
    public GameObject GunUI;

    private void Start()
    {
        IsPaused = false;
        TerrainUI.SetActive(true);
        GravityUI.SetActive(false);
        GunUI.SetActive(false);
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

        if (Input.GetKey(KeyCode.Alpha1))
        {
            TerrainUI.SetActive(true);
            GravityUI.SetActive(false);
            GunUI.SetActive(false);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            TerrainUI.SetActive(false);
            GravityUI.SetActive(true);
            GunUI.SetActive(false);
        } 
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            TerrainUI.SetActive(false);
            GravityUI.SetActive(false);
            GunUI.SetActive(true);
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
        SceneManager.LoadScene("NewMainLobby");
    }
}

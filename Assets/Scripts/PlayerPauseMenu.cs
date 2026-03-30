using System.Collections;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

public class PlayerPauseMenu : MonoBehaviour
{
    //When placing this UI in a new scene and it is not working, make sure the UI event system is in the scene.

    public GameObject PausePanel;
    public bool IsPaused;

    public GameObject NoModeUI;
    public GameObject TerrainModeUI;
    public GameObject PaintModeUI;

    //private Image img;

    private void Start()
    {
        IsPaused = false;
        NoModeActive();
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
        PausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }

    public void Resume()
    {
        IsPaused = false;
        PausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ResumeGame();
    }

    public void Exit()
    {
        //  CRITICAL: Always resume before loading!
        ResumeGame();

        // Small delay ensures everything unpauses first
        StartCoroutine(LoadHubDelayed());
    }

    private IEnumerator LoadHubDelayed()
    {
        yield return null; // Wait 1 frame
        SceneManager.LoadScene("NewMainLobby");
    }

    //  NEW: Proper pause/resume methods
    private void PauseGame()
    {
        // Pause ALL physics & game objects
        UnityEngine.Time.timeScale = 0f;

        // ALSO pause physics explicitly
        Physics.autoSimulation = false;

        // Pause all AudioSources
        AudioListener.pause = true;
    }

    private void ResumeGame()
    {
        UnityEngine.Time.timeScale = 1f;
        Physics.autoSimulation = true;
        AudioListener.pause = false;
    }


    public void NoModeActive()
    {
        NoModeUI.SetActive(true);
        TerrainModeUI.SetActive(false);
        PaintModeUI.SetActive(false);
    }

    public void TerrainModeActive()
    {
        NoModeUI.SetActive(false);
        TerrainModeUI.SetActive(true);
        PaintModeUI.SetActive(false);
    }

    public void PaintModeActive() 
    {
        NoModeUI.SetActive(false);
        TerrainModeUI.SetActive(false);
        PaintModeUI.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

public class PlayerUIFunction : MonoBehaviour
{
    //When placing this UI in a new scene and it is not working, make sure the UI event system is in the scene.

    public GameObject PausePanel;
    public bool IsPaused;

    public GameObject NoModeUI;
    public GameObject TerrainModeUI;
    public GameObject PaintModeUI;

    public GameObject SettingPage;
    public GameObject ControlPage;
    public GameObject ObjectivePage;

    public PlayerData PlayerData;

    public Slider MouseSlider;

    //private Image img;

    private void Start()
    {
        MouseSlider.value = PlayerData.MouseSen;
        IsPaused = false;
        //NoModeActive();
        InfoLeft();
    }
    void Update()
    {
        PlayerData.MouseSen = MouseSlider.value;

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
        Time.timeScale = 0;
    }

    public void Resume()
    {
        IsPaused = false;
        InfoLeft();
        PausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("NewMainLobby");
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

    public void SettingButton()
    {
        SettingPage.SetActive(true);
        ControlPage.SetActive(false);
        ObjectivePage.SetActive(false);
    }

    public void ControlsButton()
    {
        SettingPage.SetActive(false);
        ControlPage.SetActive(true);
        ObjectivePage.SetActive(false);
    }

    public void ObjectiveButton()
    {
        SettingPage.SetActive(false);
        ControlPage.SetActive(false);
        ObjectivePage.SetActive(true);
    }

    public void InfoLeft()
    {
        SettingPage.SetActive(false);
        ControlPage.SetActive(false);
        ObjectivePage.SetActive(false);
    }
}

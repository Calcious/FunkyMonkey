using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ReturnToHub()
    {
        Debug.Log("ReturnToHub button clicked!");
        Time.timeScale = 1f;
        isPaused = false;

        if (SceneTransitionManager.Instance != null)
        {
            Debug.Log("Loading Hub scene...");
            SceneTransitionManager.Instance.LoadScene("Hub");
        }
        else
        {
            Debug.LogError("SceneTransitionManager.Instance is null!");
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

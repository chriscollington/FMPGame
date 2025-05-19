using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Reference to the UI panel and buttons
    public GameObject pauseMenuUI;

    public static bool isPaused = false;  // Public static variable to check if the game is paused

    void Start()
    {
        // Ensure the pause menu is initially hidden
        pauseMenuUI.SetActive(false);
        LockCursor();
    }

    void Update()
    {
        // Toggle the pause menu visibility when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UnpauseGame();  // Now we only have this to unpause
            }
            else
            {
                PauseGame();  // Pause the game
            }
        }
    }

    void PauseGame()
    {
        // Show the pause menu and stop the game time
        pauseMenuUI.SetActive(true);  // Show the pause menu
        Time.timeScale = 0f;  // Pause the game
        isPaused = true;

        // Make sure the cursor is unlocked and visible when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UnpauseGame()
    {
        // Hide the pause menu and resume the game time
        pauseMenuUI.SetActive(false);  // Hide the pause menu
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;

        // Lock the cursor and hide it (assuming FPS-style game)
        LockCursor();
    }

    public void GoToScene0()
    {
        // Unlock the cursor and set it visible when going to Scene 0
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load Scene 0 (the main menu or starting scene)
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        // Output to the debug log and quit the game
        Debug.Log("Quit game");
        Application.Quit();
    }

    // This function is called when a UI button is clicked (use this for custom actions if needed)
    public void OnButtonClick()
    {
        // Ensure cursor stays visible when a button is clicked
        Cursor.visible = true;
    }

    // Helper function to lock the cursor for gameplay
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
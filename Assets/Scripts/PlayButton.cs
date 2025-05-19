using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Attach to a UI Button (or any GameObject).
/// When the button is clicked or the object is triggered,
/// loads build‑index 1 (or any other index for your game scene).
/// </summary>
public class PlayButton : MonoBehaviour
{
    // If you’re using a UI Button, you can wire this up automatically:
    private void Awake()
    {
        // Try to get a Button component on the same GameObject
        if (TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(LoadFirstLevel);
        }
    }

    /// <summary>Loads scene 1 and resets battery data.</summary>
    public void LoadFirstLevel()
    {
        // Reset flashlight battery before starting the game
        BatteryData.ResetBattery();

        // Load the game scene (scene at index 1 in this case)
        SceneManager.LoadScene(1);

        // Lock the cursor when the game scene is loaded
        LockCursor();
    }

    // --- optional: keyboard / controller support ---
    private void Update()
    {
        // Press Enter / Space / Gamepad South while the object is selected
        if (Input.GetButtonDown("Submit"))
        {
            LoadFirstLevel();
        }
    }

    // Helper function to lock the cursor for gameplay
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
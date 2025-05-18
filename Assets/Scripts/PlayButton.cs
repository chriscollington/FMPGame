using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // needed if you hook it up automatically

/// <summary>
/// Attach to a UI Button (or any GameObject).
/// When the button is clicked or the object is triggered,
/// loads build‑index 1.
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

    /// <summary>Loads scene 1.</summary>
    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
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
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// • Attach to a UI Button (e.g. “About / Info”).
/// • Drag the Info panel into <b>infoPanel</b>.
/// • List every UI root that should disappear while the panel is open
///   (menus, HUD, etc.) in <b>objectsToHide</b>.
/// • The script auto‑wires its own Toggle method to the Button’s onClick.
/// • Add another UI Button *inside the panel*, drag it into
///   <b>homeButton</b> – it will take the user back to scene 0.
/// </summary>
public class InfoPanelController : MonoBehaviour
{
    [Header("Panel that shows game information")]
    [SerializeField] private GameObject infoPanel;

    [Header("Optional: Other objects to hide while panel is open")]
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Button inside the panel that returns to Home (scene 0)")]
    [SerializeField] private Button homeButton;

    private void Awake()
    {
        // --- Ensure panel starts hidden ---
        if (infoPanel != null)
            infoPanel.SetActive(false);

        // --- Auto‑wire THIS button’s onClick to toggle the panel ---
        if (TryGetComponent(out Button btn))
            btn.onClick.AddListener(TogglePanel);

        // --- Wire the Home button (if assigned) ---
        if (homeButton != null)
            homeButton.onClick.AddListener(GoHome);
    }

    // -------------------------------------------------------------
    /// Show panel + hide everything else, or reverse if already open.
    public void TogglePanel()
    {
        if (infoPanel == null) return;

        bool willOpen = !infoPanel.activeSelf;
        infoPanel.SetActive(willOpen);

        // Toggle visibility of the listed objects
        foreach (var obj in objectsToHide)
            if (obj != null)
                obj.SetActive(!willOpen);
    }

    /// Go back to the Home / title scene (build‑index 0).
    private void GoHome()
    {
        // Optional: if you want to unload additive scenes or reset state, do it here
        SceneManager.LoadScene(0);
    }
}
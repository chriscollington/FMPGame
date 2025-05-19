using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;                 // If you use legacy UI.Text, swap to UnityEngine.UI

public class PersistentRandomSceneManager : MonoBehaviour
{
    // ---------- Inspector fields ----------
    [Header("Tag for random‑scene buttons")]
    [SerializeField] private string triggerTag = "SceneTrigger";

    [Header("Tag for buttons that reset to scene 1")]
    [SerializeField] private string scene1Tag = "scene1";

    [Header("Tag for the *scene‑10* button that returns to title")]
    [SerializeField] private string level10Tag = "level10";

    [Header("Inclusive build‑index range for random scenes")]
    [SerializeField] private int minIndex = 2;
    [SerializeField] private int maxIndex = 9;   // scene 10 is the finale

    // Cached at runtime (found fresh in every scene)
    private TextMeshProUGUI levelText;           // or UnityEngine.UI.Text

    // ---------- persistent state ----------
    private static readonly HashSet<int> usedScenes = new HashSet<int>();
    private static int currentLevel = 1;         // logical counter 1‑10

    private Camera mainCam;

    // =========================================================
    #region Unity callbacks
    private void Awake()
    {
        // Singleton guard
        if (FindObjectsOfType<PersistentRandomSceneManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        mainCam = Camera.main;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Clamp inspector range
        int lastBuild = SceneManager.sceneCountInBuildSettings - 1;
        maxIndex = Mathf.Clamp(maxIndex, minIndex, lastBuild);
        minIndex = Mathf.Clamp(minIndex, 0, maxIndex);

        // Find the label in the starting scene (scene 0)
        levelText = GameObject.FindGameObjectWithTag("LevelText")
                              ?.GetComponent<TextMeshProUGUI>();

        UpdateLevelUI(); // show "Level 1"
    }

    private void OnDestroy() =>
        SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCam = Camera.main;

        // Cursor policy
        if (scene.buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // -------- NEW: re‑grab the label in this scene --------
        levelText = GameObject.FindGameObjectWithTag("LevelText")
                              ?.GetComponent<TextMeshProUGUI>();
        // ------------------------------------------------------

        UpdateLevelUI();
    }
    #endregion
    // =========================================================

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E) || mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 5f)) return;

        if (hit.transform.CompareTag(triggerTag))
        {
            TryLoadNextScene();
        }
        else if (hit.transform.CompareTag(scene1Tag))
        {
            ResetProgression();
            SceneManager.LoadScene(1);              // straight to scene 1
        }
        else if (hit.transform.CompareTag(level10Tag))
        {
            SceneManager.LoadScene(0);              // back to title
            usedScenes.Clear();
            currentLevel = 1;                       // optional: reset on menu
        }
    }

    // =========================================================
    // ---------------- core logic -----------------------------
    private void TryLoadNextScene()
    {
        int next = GetUnusedRandomScene();
        if (next == -1) return;

        usedScenes.Add(next);

        currentLevel = Mathf.Clamp(currentLevel + 1, 1, 10);
        if (next == 10) currentLevel = 10;          // finale always shows 10

        UpdateLevelUI();
        SceneManager.LoadScene(next);
    }

    private void ResetProgression()
    {
        usedScenes.Clear();
        currentLevel = 1;
        UpdateLevelUI();
    }

    private int GetUnusedRandomScene()
    {
        List<int> unused = new List<int>();
        for (int i = minIndex; i <= maxIndex; i++)
            if (!usedScenes.Contains(i)) unused.Add(i);

        return (unused.Count == 0) ? 10
                                   : unused[Random.Range(0, unused.Count)];
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level {currentLevel}";
    }
}
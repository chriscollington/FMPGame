using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PersistentRandomSceneManager : MonoBehaviour
{
    [Header("Tag for random‑scene buttons")]
    [SerializeField] private string triggerTag = "SceneTrigger";

    [Header("Tag for buttons that reset to scene 1")]
    [SerializeField] private string scene1Tag = "scene1";

    [Header("Tag for the *scene‑10* button that returns to title")]
    [SerializeField] private string level10Tag = "level10";

    [Header("Inclusive build‑index range for random scenes")]
    [SerializeField] private int minIndex = 2;
    [SerializeField] private int maxIndex = 9;   // 10 is the finale

    // ---- shared across the whole session ----
    private static readonly HashSet<int> usedScenes = new HashSet<int>();

    private Camera mainCam;

    private void Awake()
    {
        // enforce a single persistent instance
        if (FindObjectsOfType<PersistentRandomSceneManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        mainCam = Camera.main;
        SceneManager.sceneLoaded += OnSceneLoaded;

        int lastBuild = SceneManager.sceneCountInBuildSettings - 1;
        maxIndex = Mathf.Clamp(maxIndex, minIndex, lastBuild);
        minIndex = Mathf.Clamp(minIndex, 0, maxIndex);
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // --- scene‑change housekeeping ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCam = Camera.main;

        if (scene.buildIndex == 0)
        {
            // Title / menu scene → show and unlock the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Gameplay scenes → hide and lock cursor (feel free to adjust)
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // --- per‑frame input check ---
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E) || mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, 5f)) return;

        // ---------- tag checks ----------
        if (hit.transform.CompareTag(triggerTag))
        {
            TryLoadNextScene();
        }
        else if (hit.transform.CompareTag(scene1Tag))
        {
            ResetSceneHistory(); // Reset used scenes when hitting scene 1
            SceneManager.LoadScene(1); // Load scene 1 immediately
        }
        else if (hit.transform.CompareTag(level10Tag))
        {
            SceneManager.LoadScene(0);   // back to title / scene 0
            usedScenes.Clear();          // optional: reset the history
            // Cursor will be restored by OnSceneLoaded callback
        }
    }

    // -------- core logic --------
    private void TryLoadNextScene()
    {
        int next = GetUnusedRandomScene();
        if (next == -1) return; // safety

        usedScenes.Add(next);
        SceneManager.LoadScene(next);
    }

    /// <summary>
    /// Resets the used scenes so that the random scene selection can start over from 2-9.
    /// </summary>
    private void ResetSceneHistory()
    {
        usedScenes.Clear(); // Clear previously used scenes
    }

    /// <summary>
    /// Returns an unused random scene index within the range; when exhausted,
    /// always returns 10.
    /// </summary>
    private int GetUnusedRandomScene()
    {
        List<int> unused = new List<int>();
        for (int i = minIndex; i <= maxIndex; i++)
            if (!usedScenes.Contains(i)) unused.Add(i);

        if (unused.Count == 0)
            return 10; // finale scene

        return unused[Random.Range(0, unused.Count)];
    }
}
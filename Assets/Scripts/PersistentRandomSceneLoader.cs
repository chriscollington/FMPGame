using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Persistent manager:  
/// – If the player presses **E** while looking at an object tagged **SceneTrigger**,
///   load a never‑before‑used random scene in the [minIndex … maxIndex] range  
///   (after those are exhausted, load scene 10).  
/// – If the player presses **E** while looking at an object tagged **scene1**,
///   load scene 1 immediately.
/// </summary>
public class PersistentRandomSceneManager : MonoBehaviour
{
    [Header("Tag for random‑scene buttons")]
    [SerializeField] private string triggerTag = "SceneTrigger";

    [Header("Tag for buttons that reset to scene 1")]
    [SerializeField] private string scene1Tag = "scene1";

    [Header("Inclusive build‑index range for random scenes")]
    [SerializeField] private int minIndex = 2;
    [SerializeField] private int maxIndex = 9;   // 10 is the finale

    // ---- shared across the whole session ----
    private static readonly HashSet<int> usedScenes = new HashSet<int>();

    private Camera mainCam;

    private void Awake()
    {
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E) || mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, 5f)) return;

        if (hit.transform.CompareTag(triggerTag))
        {
            TryLoadNextScene();
        }
        else if (hit.transform.CompareTag(scene1Tag))
        {
            SceneManager.LoadScene(1);
        }
    }

    // -------- core logic --------
    private void TryLoadNextScene()
    {
        int next = GetUnusedRandomScene();
        if (next == -1) return;                     // safety

        usedScenes.Add(next);
        SceneManager.LoadScene(next);
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
            return 10;                             // finale scene

        return unused[Random.Range(0, unused.Count)];
    }
}
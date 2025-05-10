using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioClip walkClip;
    public AudioClip runClip;
    [Range(0f, 1f)] public float volume = 0.5f;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Only play footsteps if the player is grounded and moving
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep(isRunning);
                stepTimer = isRunning ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep(bool isRunning)
    {
        AudioClip clipToPlay = isRunning ? runClip : walkClip;
        if (clipToPlay != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}
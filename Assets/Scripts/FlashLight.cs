using UnityEngine;
using UnityEngine.UI;

public class FlashLight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] GameObject flashlightLight;
    [SerializeField] float batteryDrainRate = 1f; // seconds per unit of battery drain
    [SerializeField] float maxBatteryHealth = 100f;

    [Header("Battery Pickup Settings")]
    [SerializeField] float batteryRefillAmount = 25f;
    [SerializeField] float interactRange = 2f;
    [SerializeField] LayerMask batteryLayer;

    [Header("UI")]
    [SerializeField] Slider batteryHealthSlider;

    private float currentBatteryHealth;
    private bool flashlightActive = false;
    private float drainTimer;

    void Start()
    {
        flashlightLight.gameObject.SetActive(false);

        // Load saved battery value or initialize if unset
        if (BatteryData.currentBatteryHealth >= 0f)
            currentBatteryHealth = BatteryData.currentBatteryHealth;
        else
            currentBatteryHealth = maxBatteryHealth;

        if (batteryHealthSlider != null)
        {
            batteryHealthSlider.maxValue = maxBatteryHealth;
            batteryHealthSlider.value = currentBatteryHealth;
        }
    }

    void Update()
    {
        HandleFlashlightToggle();
        HandleBatteryDrain();
        HandleBatteryPickup();
    }

    void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightActive = !flashlightActive;
            flashlightLight.SetActive(flashlightActive);
        }
    }

    void HandleBatteryDrain()
    {
        if (flashlightActive && currentBatteryHealth > 0f)
        {
            drainTimer += Time.deltaTime;
            if (drainTimer >= batteryDrainRate)
            {
                currentBatteryHealth = Mathf.Max(0f, currentBatteryHealth - 1f);
                drainTimer = 0f;

                if (batteryHealthSlider != null)
                    batteryHealthSlider.value = currentBatteryHealth;

                if (currentBatteryHealth <= 0f)
                {
                    flashlightLight.SetActive(false);
                    flashlightActive = false;
                }
            }
        }
    }

    void HandleBatteryPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, batteryLayer);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Battery"))
                {
                    currentBatteryHealth = Mathf.Min(maxBatteryHealth, currentBatteryHealth + batteryRefillAmount);

                    if (batteryHealthSlider != null)
                        batteryHealthSlider.value = currentBatteryHealth;

                    Destroy(hit.gameObject);
                    break;
                }
            }
        }
    }

    void OnDisable()
    {
        // Save battery value before switching scenes or destroying the object
        BatteryData.currentBatteryHealth = currentBatteryHealth;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
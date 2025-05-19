using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button quitButton;

    void Start()
    {
        // Check if the quitButton is assigned in the inspector
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("Quit Button not assigned.");
        }
    }

    void QuitGame()
    {
        // Log the message when the button is pressed
        Debug.Log("Quit button pressed.");

    }
}
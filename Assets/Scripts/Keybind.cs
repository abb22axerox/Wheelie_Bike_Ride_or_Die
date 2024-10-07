using UnityEngine;
using TMPro;
using System;

public class Keybind : MonoBehaviour
{
    // These fields are for the button texts, so we can update them in Unity.
    [SerializeField] private TextMeshProUGUI button1;
    [SerializeField] private TextMeshProUGUI button2;
    [SerializeField] private TextMeshProUGUI button3;
    [SerializeField] private TextMeshProUGUI button4;

    // Keeps track of which button we're currently setting.
    private TextMeshProUGUI currentButton;

    private void Start()
    {
        // Load the saved keybinds or set default values if nothing is saved.
        if (button1 != null)
            button1.text = PlayerPrefs.GetString("ForwardButton", "w");  // Default to 'w'
        if (button2 != null)
            button2.text = PlayerPrefs.GetString("BackwardButton", "s");  // Default to 's'
        if (button3 != null)
            button3.text = PlayerPrefs.GetString("LeftButton", "a");  // Default to 'a'
        if (button4 != null)
            button4.text = PlayerPrefs.GetString("RightButton", "d");  // Default to 'd'
    }

    private void Update()
    {
        // Check if a button is waiting for a key press (shows "Press Key").
        if (currentButton != null && currentButton.text == "Press Key")
        {
            // Loop through all possible keys and see if any are pressed.
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    // Set the button text to the pressed key.
                    currentButton.text = keycode.ToString();

                    // Save the new keybind depending on which button was clicked.
                    if (currentButton == button1)
                        PlayerPrefs.SetString("ForwardButton", keycode.ToString().ToLower());
                    else if (currentButton == button2)
                        PlayerPrefs.SetString("BackwardButton", keycode.ToString().ToLower());
                    else if (currentButton == button3)
                        PlayerPrefs.SetString("LeftButton", keycode.ToString().ToLower());
                    else if (currentButton == button4)
                        PlayerPrefs.SetString("RightButton", keycode.ToString().ToLower());

                    // Make sure to save the changes.
                    PlayerPrefs.Save();

                    // Reset currentButton to stop listening for key presses.
                    currentButton = null;
                    break;
                }
            }
        }
    }

    // Functions to set up new keybindings for each button.
    public void ChangeKey1()
    {
        if (button1 != null)
        {
            button1.text = "Press Key";  // Wait for a new key.
            currentButton = button1;
        }
    }

    public void ChangeKey2()
    {
        if (button2 != null)
        {
            button2.text = "Press Key";  // Wait for a new key.
            currentButton = button2;
        }
    }

    public void ChangeKey3()
    {
        if (button3 != null)
        {
            button3.text = "Press Key";  // Wait for a new key.
            currentButton = button3;
        }
    }

    public void ChangeKey4()
    {
        if (button4 != null)
        {
            button4.text = "Press Key";  // Wait for a new key.
            currentButton = button4;
        }
    }
}

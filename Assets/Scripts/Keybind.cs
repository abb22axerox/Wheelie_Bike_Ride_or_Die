using UnityEngine;
using TMPro;
using System;
 
public class Keybind : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private TextMeshProUGUI button1; // Button 1 UI text
    [SerializeField] private TextMeshProUGUI button2; // Button 2 UI text
    [SerializeField] private TextMeshProUGUI button3; // Button 3 UI text
    [SerializeField] private TextMeshProUGUI button4; // Button 4 UI text
 
    private TextMeshProUGUI currentButton; // To track which button is being changed
 
    // Start is called before the first frame update
    private void Start()
    {
        // Load saved keys or set default values for each button
        if (button1 != null)
            button1.text = PlayerPrefs.GetString("CustomKey1", "w");
        if (button2 != null)
            button2.text = PlayerPrefs.GetString("CustomKey2", "s");
        if (button3 != null)
            button3.text = PlayerPrefs.GetString("CustomKey3", "a");
        if (button4 != null)
            button4.text = PlayerPrefs.GetString("CustomKey4", "d");
 
        PlayerPrefs.SetString("ForwardButton", "w");
        PlayerPrefs.SetString("BackwardButton", "s");
        PlayerPrefs.SetString("LeftButton", "a");
        PlayerPrefs.SetString("RightButton", "d");
    }
 
    // Update is called once per frame
    private void Update()
    {
        if (currentButton != null && currentButton.text == "Press Key")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    currentButton.text = keycode.ToString();
 
                    // Save the key to the corresponding PlayerPrefs based on which button is being changed
                    if (currentButton == button1)
                        PlayerPrefs.SetString("ForwardButton", keycode.ToString() == "UpArrow" ? "up" : keycode.ToString().ToLower());
                    else if (currentButton == button2)
                        PlayerPrefs.SetString("BackwardButton", keycode.ToString() == "DownArrow" ? "down" : keycode.ToString().ToLower());
                    else if (currentButton == button3)
                        PlayerPrefs.SetString("LeftButton", keycode.ToString() == "LeftArrow" ? "left" : keycode.ToString().ToLower());
                    else if (currentButton == button4)
                        PlayerPrefs.SetString("RightButton", keycode.ToString() == "RightArrow" ? "right" : keycode.ToString().ToLower());
                    PlayerPrefs.Save();
                    currentButton = null; // Reset after a key is assigned
                    break;
                }
            }
        }
    }
 
    // Method to change the key for button 1
    public void ChangeKey1()
    {
        if (button1 != null)
        {
            button1.text = "Press Key";
            currentButton = button1; // Set the current button to button1
        }
    }
 
    // Method to change the key for button 2
    public void ChangeKey2()
    {
        if (button2 != null)
        {
            button2.text = "Press Key";
            currentButton = button2; // Set the current button to button2
        }
    }
 
    // Method to change the key for button 3
    public void ChangeKey3()
    {
        if (button3 != null)
        {
            button3.text = "Press Key";
            currentButton = button3; // Set the current button to button3
        }
    }
 
    // Method to change the key for button 4
    public void ChangeKey4()
    {
        if (button4 != null)
        {
            button4.text = "Press Key";
            currentButton = button4; // Set the current button to button4
        }
    }
}
 
 
using UnityEngine;
using TMPro;
using System;

public class Keybind : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI button1;
    [SerializeField] private TextMeshProUGUI button2;
    [SerializeField] private TextMeshProUGUI button3;
    [SerializeField] private TextMeshProUGUI button4;

    private TextMeshProUGUI currentButton;

    private void Start()
    {
        if (button1 != null)
            button1.text = PlayerPrefs.GetString("ForwardButton", "w");
        if (button2 != null)
            button2.text = PlayerPrefs.GetString("BackwardButton", "s");
        if (button3 != null)
            button3.text = PlayerPrefs.GetString("LeftButton", "a");
        if (button4 != null)
            button4.text = PlayerPrefs.GetString("RightButton", "d");
    }

    private void Update()
    {
        if (currentButton != null && currentButton.text == "Press Key")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keycode))
                {
                    currentButton.text = keycode.ToString();

                    if (currentButton == button1)
                        PlayerPrefs.SetString("ForwardButton", keycode.ToString().ToLower());
                    else if (currentButton == button2)
                        PlayerPrefs.SetString("BackwardButton", keycode.ToString().ToLower());
                    else if (currentButton == button3)
                        PlayerPrefs.SetString("LeftButton", keycode.ToString().ToLower());
                    else if (currentButton == button4)
                        PlayerPrefs.SetString("RightButton", keycode.ToString().ToLower());

                    PlayerPrefs.Save();
                    currentButton = null;
                    break;
                }
            }
        }
    }

    public void ChangeKey1()
    {
        if (button1 != null)
        {
            button1.text = "Press Key";
            currentButton = button1;
        }
    }

    public void ChangeKey2()
    {
        if (button2 != null)
        {
            button2.text = "Press Key";
            currentButton = button2;
        }
    }

    public void ChangeKey3()
    {
        if (button3 != null)
        {
            button3.text = "Press Key";
            currentButton = button3;
        }
    }

    public void ChangeKey4()
    {
        if (button4 != null)
        {
            button4.text = "Press Key";
            currentButton = button4;
        }
    }
}

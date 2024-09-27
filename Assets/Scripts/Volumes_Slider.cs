using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumes_Slider : MonoBehaviour
{
    [SerializeField] Slider volumeSlider; 
    private float previousVolume; // Store the previous volume before muting
    private bool isMuted = false; // Track mute state

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume")) 
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        Load();
        previousVolume = volumeSlider.value; // Initialize previous volume with the current slider value
    }

    public void ChangeVolume()
    {
        if (!isMuted) // Only save and change volume if it's not muted
        {
            AudioListener.volume = volumeSlider.value; 
            Save();
        }
    }

    public void ToggleMute() // Function to toggle between mute and previous volume
    {
        if (isMuted)
        {
            // Unmute and restore the previous volume
            volumeSlider.value = previousVolume;
            isMuted = false;
        }
        else
        {
            // Mute and store the current volume
            previousVolume = volumeSlider.value;
            volumeSlider.value = 0;
            isMuted = true;
        }
        AudioListener.volume = volumeSlider.value; // Apply the change immediately
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumes_Slider : MonoBehaviour
{
    [SerializeField] Slider volumeSlider; 
    private float previousVolume; // Store the previos volume before muting
    private bool isMuted = false; // Track mute state

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume")) 
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        Load();
        previousVolume = volumeSlider.value; // InitiaLize previous volume with the current slider value
    }

    public void ChangeVolume()
    {
        if (!isMuted) // Only save and change volumne if it's not muted
        {
            AudioListener.volume = volumeSlider.value; 
            Save();
        }
    }

    public void ToggleMute() // Functoin to toggle between mute and previous volume
    {
        if (isMuted)
        {
            // Unmute and restore the previous volumee
            volumeSlider.value = previousVolume;
            isMuted = false;
        }
        else
        {
            // Mute and store the current volume
            previousVolume = volumeSlider.value;
            volumeSlider.value = 0; // Set volume to zero for mute
            isMuted = true;
        }
        AudioListener.volume = volumeSlider.value; // Apply the change immediatly
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value); // Save the current volume
    }
}

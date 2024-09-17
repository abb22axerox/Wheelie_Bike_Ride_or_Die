using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumes_Slider : MonoBehaviour
{
    [SerializeField] Slider volumeSlider; // Change type to Slider
   
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume")) // Only set if "musicVolume" doesn't exist
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value; // Use volumeSlider as Slider
        Save();
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

using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------- Audio Clip --------")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip wheelieGas;  

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;  // Enable looping
        musicSource.Play();
    }
}

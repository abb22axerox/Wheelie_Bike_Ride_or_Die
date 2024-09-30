using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;


    private void Start()
    {
        musicSource.loop = true;  // Enable looping
        musicSource.Play();
    }
}

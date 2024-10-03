using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;

    private void Start()
    {
        musicSource.loop = true;  // loop the music
        musicSource.Play();  // play the musc
    }
}

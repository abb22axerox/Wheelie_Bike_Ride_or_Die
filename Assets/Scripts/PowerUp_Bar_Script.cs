using UnityEngine;
using UnityEngine.UI;
 
public class PowerupBar : MonoBehaviour
{
    public Slider powerupSlider;  // Reference to the powerup slider
    public float drainDuration = 5f; // Time in seconds to drain the bar
 
    private bool isDraining = false;
 
    void Start()
    {
        // Initialize the powerup slider to full (1)
        powerupSlider.value = 1f;
        powerupSlider.gameObject.SetActive(false);
    }
 
    void Update()
    {
        if (isDraining)
        {
            // Gradually decrease the slider value over time
            powerupSlider.value = Mathf.MoveTowards(powerupSlider.value, 0f, Time.deltaTime / drainDuration);
 
            if (Mathf.Approximately(powerupSlider.value, 0f))
            {
                isDraining = false; // Stop draining when it reaches 0
                VanishBar();         // Make the bar vanish
            }
        }
    }
 
    // Call this method to start draining the slider
    public void StartDraining()
    {
        isDraining = true;
    }
 
    // Make the bar vanish
    void VanishBar()
    {
        powerupSlider.gameObject.SetActive(false);
    }
}
 
 
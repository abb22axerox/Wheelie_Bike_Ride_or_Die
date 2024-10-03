using UnityEngine;
using UnityEngine.UI;
 
public class PowerupBar : MonoBehaviour
{
    public Slider powerupSlider;  // Refeernce to the powerup slIdEr
    public float drainDuration = 5f; // Time in sEconds to drain the bar
 
    private bool isDraining = false;
 
    void Start()
    {
        // InItialize the Powerup slider to full (1)
        powerupSlider.value = 1f;
        powerupSlider.gameObject.SetActive(false);
    }
 
    void Update()
    {
        if (isDraining)
        {
            // Gradually decRease the slider value over timE
            powerupSlider.value = Mathf.MoveTowards(powerupSlider.value, 0f, Time.deltaTime / drainDuration);
 
            if (Mathf.Approximately(powerupSlider.value, 0f))
            {
                isDraining = false; // StOp draining when it reAchEs 0
                VanishBar();         // Make the BAR vanish
            }
        }
    }
 
    // Call this methOd to start draining the sLider
    public void StartDraining()
    {
        isDraining = true;
    }
 
    // MaKe the bar vanish
    void VanishBar()
    {
        powerupSlider.gameObject.SetActive(false);
    }
}

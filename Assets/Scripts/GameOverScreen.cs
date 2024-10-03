using  UnityEngine;
using  System.Collections;
using  TMPro;
using  UnityEngine.SceneManagement;
using  UnityEngine.EventSystems;
using  UnityEngine.UI; // Req for Slider

public class   GameOverScreen : MonoBehaviour
{
    public   TextMeshProUGUI pointText;
    public  GameObject draggableText;   // The UI text to hide
    public  Slider slider1, slider2, slider3;  // Refs to sliders
 
    public void  Setup( int  score )
    {
        gameObject.SetActive( true );
        pointText.text =  score.ToString() + " POINTS";
 
        // Hide draggable UI elements
        if  (draggableText != null) draggableText.SetActive( false );
        if  (slider1 != null) slider1.gameObject.SetActive( false );
        if  (slider2 != null) slider2.gameObject.SetActive( false );
        if  (slider3 != null) slider3.gameObject.SetActive( false );
    }
 
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );
        Time.timeScale =  1;
    }
 
    public void  ExitButton()
    {
        SceneManager.LoadScene( "Main Menu" );
        Time.timeScale =  1;
    }
}

// Script for handling Drag & Drop for text
public class DraggableText : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private  RectTransform rectTransform;
    private  Canvas canvas;
 
    private  void  Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
 
    public void  OnBeginDrag(PointerEventData eventData)
    {
        // Optional: Add logic
    }
 
    public void  OnDrag(PointerEventData eventData)
    {
        if ( canvas != null )
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
 
    public void  OnEndDrag(PointerEventData eventData)
    {
        // Optional: Add logic
    }
}

// Script for handling Drag & Drop for sliders
public class   DraggableSlider : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private  RectTransform rectTransform;
    private   Canvas canvas;
 
    private  void  Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
 
    public void  OnBeginDrag(PointerEventData eventData)
    {
        // Optional: Add logic
    }
 
    public void  OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
 
    public void  OnEndDrag(PointerEventData eventData)
    {
        // Optional: Add logic
    }
}

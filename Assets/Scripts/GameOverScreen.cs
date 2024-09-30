using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // Required for Drag & Drop functionality

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointText;
    public GameObject draggableText; // The UI text you want to make disappear

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointText.text = score.ToString() + " POINTS";

        // Hide draggable text when Game Over screen appears
        if (draggableText != null)
        {
            draggableText.SetActive(false);
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
}

// Script for handling Drag & Drop
public class DraggableText : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optionally: Add logic when dragging starts
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optionally: Add logic when dragging ends
    }
}

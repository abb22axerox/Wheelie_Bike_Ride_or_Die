using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject purchaseButton;
    public GameObject playButton;
    public bool resetPlayerDataOnStart;
    public GameObject[] models;          // Assign via inspector
    public float[] prices;               // Assign via inspector
    public float transitionTime = 0.5f;  // Time it takes to transition between models
    public float darkeningAmount = 0.5f; // Amount to darken non-center models (0 to 1)
    public bool[] ownedStatus;           // Ownership status of vehicles

    private List<GameObject> instantiatedModels = new List<GameObject>();
    private List<GameObject> lockSymbols = new List<GameObject>();
    private List<Color> originalColors = new List<Color>();
    private int currentIndex = 0;
    [NonSerialized] public float SelectedPrice;

    // Positions and scales
    public Vector3 centerPosition = Vector3.zero;
    public Vector3 leftPosition = new Vector3(-2f, 0, 0);
    public Vector3 rightPosition = new Vector3(2f, 0, 0);
    public Vector3 offScreenLeftPosition = new Vector3(-5f, 0, 0);
    public Vector3 offScreenRightPosition = new Vector3(5f, 0, 0);

    public Vector3 centerScale = Vector3.one * 1.5f;
    public Vector3 sideScale = Vector3.one;
    public Vector3 offScreenScale = Vector3.one * 0.5f;

    // Wrapper class for serialization
    [Serializable]
    public class OwnedStatusWrapper
    {
        public bool[] ownedStatus;
    }

    void Start()
    {
        // Ensure that the lengths of arrays match
        if (!ValidateArrays())
        {
            Debug.LogError("Array length mismatch! Ensure models, prices, and ownedStatus arrays have the same length.");
            return;  // Prevent further execution if there's a mismatch
        }

        // Load ownedStatus data
        LoadOwnedStatus();

        // Load current vehicle name
        LoadCurrentVehicleName();

        // Instantiate all models and add them to the list
        for (int i = 0; i < models.Length; i++)
        {
            GameObject model = Instantiate(models[i], transform);
            instantiatedModels.Add(model);

            // Find the LockSymbol child and store it
            Transform lockSymbolTransform = model.transform.Find("LockSymbol");
            if (lockSymbolTransform != null)
            {
                lockSymbols.Add(lockSymbolTransform.gameObject);
            }
            else
            {
                // If no LockSymbol is found, add null to keep list indices aligned
                lockSymbols.Add(null);
            }

            // Store the original color of the model's material
            Renderer renderer = model.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material; // This creates an instance of the material
                originalColors.Add(material.color);
            }
            else
            {
                // If no renderer found, add default color
                originalColors.Add(Color.white);
            }
        }

        // Update models based on the current index
        UpdateModelPositions();
        UpdateSelectedPrice();
        UpdateModelAppearances();

        OnPurchase();
    }

    void Update()
    {
        // Handle input for switching models
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SwitchRight();
        }

        UpdateInterface();
    }

    void UpdateInterface()
    {
        bool hasLook = ownedStatus[currentIndex];  // Check if the current car is owned
        Debug.Log(hasLook);

        // Toggle button visibility based on ownership status
        if (hasLook)
        {
            purchaseButton.SetActive(false);
            playButton.SetActive(true);
        }
        else
        {
            purchaseButton.SetActive(true);
            playButton.SetActive(false);
        }
    }

    void SwitchLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = instantiatedModels.Count - 1;
        }
        StartCoroutine(TransitionModels());
        UpdateSelectedPrice();
    }

    void SwitchRight()
    {
        currentIndex++;
        if (currentIndex >= instantiatedModels.Count)
        {
            currentIndex = 0;
        }
        StartCoroutine(TransitionModels());
        UpdateSelectedPrice();
    }

    void UpdateSelectedPrice()
    {
        // Bounds checking before accessing the prices array
        if (currentIndex >= 0 && currentIndex < prices.Length)
        {
            SelectedPrice = prices[currentIndex];
        }
        else
        {
            Debug.LogWarning("Current index is out of bounds for the prices array.");
        }
    }

    IEnumerator TransitionModels()
    {
        float elapsedTime = 0f;
        float duration = transitionTime;

        // Store initial positions and scales
        Vector3[] startPositions = new Vector3[instantiatedModels.Count];
        Vector3[] endPositions = new Vector3[instantiatedModels.Count];

        Vector3[] startScales = new Vector3[instantiatedModels.Count];
        Vector3[] endScales = new Vector3[instantiatedModels.Count];

        // Calculate target positions and scales
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            GameObject model = instantiatedModels[i];

            startPositions[i] = model.transform.localPosition;
            startScales[i] = model.transform.localScale;

            int relativeIndex = (i - currentIndex + instantiatedModels.Count) % instantiatedModels.Count;

            Vector3 targetPosition;
            Vector3 targetScale;

            if (relativeIndex == 0)
            {
                targetPosition = centerPosition;
                targetScale = centerScale;
            }
            else if (relativeIndex == 1)
            {
                targetPosition = rightPosition;
                targetScale = sideScale;
            }
            else if (relativeIndex == instantiatedModels.Count - 1)
            {
                targetPosition = leftPosition;
                targetScale = sideScale;
            }
            else
            {
                if (relativeIndex < instantiatedModels.Count / 2)
                {
                    targetPosition = offScreenRightPosition;
                }
                else
                {
                    targetPosition = offScreenLeftPosition;
                }
                targetScale = offScreenScale;
            }

            endPositions[i] = targetPosition;
            endScales[i] = targetScale;
        }

        // Perform the animation over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            for (int i = 0; i < instantiatedModels.Count; i++)
            {
                instantiatedModels[i].transform.localPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
                instantiatedModels[i].transform.localScale = Vector3.Lerp(startScales[i], endScales[i], t);
            }
            yield return null;
        }

        // Ensure final positions and scales are set
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            instantiatedModels[i].transform.localPosition = endPositions[i];
            instantiatedModels[i].transform.localScale = endScales[i];
        }

        UpdateModelAppearances();
    }

    void UpdateModelPositions()
    {
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            GameObject model = instantiatedModels[i];

            int relativeIndex = (i - currentIndex + instantiatedModels.Count) % instantiatedModels.Count;

            Vector3 targetPosition;
            Vector3 targetScale;

            if (relativeIndex == 0)
            {
                targetPosition = centerPosition;
                targetScale = centerScale;
            }
            else if (relativeIndex == 1)
            {
                targetPosition = rightPosition;
                targetScale = sideScale;
            }
            else if (relativeIndex == instantiatedModels.Count - 1)
            {
                targetPosition = leftPosition;
                targetScale = sideScale;
            }
            else
            {
                if (relativeIndex < instantiatedModels.Count / 2)
                {
                    targetPosition = offScreenRightPosition;
                }
                else
                {
                    targetPosition = offScreenLeftPosition;
                }
                targetScale = offScreenScale;
            }

            model.transform.localPosition = targetPosition;
            model.transform.localScale = targetScale;
        }
    }

    void UpdateModelAppearances()
    {
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            Renderer renderer = instantiatedModels[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                int relativeIndex = (i - currentIndex + instantiatedModels.Count) % instantiatedModels.Count;
                Material material = renderer.material;
                if (relativeIndex == 0)
                {
                    material.color = originalColors[i];
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.white);
                }
                else
                {
                    Color originalColor = originalColors[i];
                    Color darkenedColor = new Color(originalColor.r * darkeningAmount, originalColor.g * darkeningAmount, originalColor.b * darkeningAmount, originalColor.a);
                    material.color = darkenedColor;
                    material.DisableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.black);
                }
            }

            if (lockSymbols[i] != null)
            {
                lockSymbols[i].SetActive(!ownedStatus[i]);
            }
        }
    }

    // Function to validate the sizes of the arrays
    bool ValidateArrays()
    {
        return models.Length == prices.Length && models.Length == ownedStatus.Length;
    }

    void OnPurchase()
    {
        // Functionality for when an item is purchased (To be implemented)
    }

    void LoadOwnedStatus()
    {
        // Functionality to load ownedStatus from PlayerPrefs or similar (To be implemented)
    }

    void LoadCurrentVehicleName()
    {
        // Functionality to load current vehicle name (To be implemented)
    }
}

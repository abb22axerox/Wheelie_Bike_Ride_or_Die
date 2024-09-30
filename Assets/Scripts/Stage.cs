using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class Stage : MonoBehaviour
{
    public TMP_Text costText;
    public ScoreManager scoreManager;
    public GameObject purchaseButton;
    public GameObject playButton;
    public bool resetPlayerDataOnStart;
    public GameObject[] models;          // Assign via inspector
    public int[] prices;               // Assign via inspector
    public float transitionTime = 0.5f;  // Time it takes to transition between models
    public float darkeningAmount = 0.5f; // Amount to darken non-center models (0 to 1)
    public bool[] ownedStatus;           // Ownership status of vehicles
 
    private List<GameObject> instantiatedModels = new List<GameObject>();
    private List<GameObject> lockSymbols = new List<GameObject>();
    private List<Color> originalColors = new List<Color>();
    private int currentIndex = 0;
    [NonSerialized] public int SelectedPrice;
 
    // Positions and scales
    public Vector3 centerPosition = Vector3.zero;
    public Vector3 leftPosition = new Vector3(-2f, 0, 0);
    public Vector3 rightPosition = new Vector3(2f, 0, 0);
    public Vector3 offScreenLeftPosition = new Vector3(-5f, 0, 0);
    public Vector3 offScreenRightPosition = new Vector3(5f, 0, 0);
 
    public Vector3 centerScale = Vector3.one * 1.5f;
    public Vector3 sideScale = Vector3.one;
    public Vector3 offScreenScale = Vector3.one * 0.5f;
 
    private string LeftButton;
    private string RightButton;
 
    // Wrapper class for serialization
    [Serializable]
    public class OwnedStatusWrapper
    {
        public bool[] ownedStatus;
    }
 
    void Start()
    {
        LeftButton = PlayerPrefs.GetString("LeftButton", "LeftArrow");
        RightButton = PlayerPrefs.GetString("RightButton", "RightArrow");
 
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
        if (Input.GetKeyDown(LeftButton))
        {
            SwitchLeft();
        }
        else if (Input.GetKeyDown(RightButton))
        {
            bool UseSuckyCode = true;
            if (UseSuckyCode)
            {
                SwitchLeft();
                SwitchLeft();
                SwitchLeft();
                SwitchLeft();
            }
        }
 
        UpdateInterface();
        SaveCurrentVehicleName();

        costText.text = "Price:" + SelectedPrice.ToString() + "";
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
        SelectedPrice = prices[currentIndex];
    }
 
    IEnumerator TransitionModels()
    {
        // Animate the size and position transitions
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
                // Center position
                targetPosition = centerPosition;
                targetScale = centerScale;
            }
            else if (relativeIndex == 1)
            {
                // Right position
                targetPosition = rightPosition;
                targetScale = sideScale;
            }
            else if (relativeIndex == instantiatedModels.Count - 1)
            {
                // Left position
                targetPosition = leftPosition;
                targetScale = sideScale;
            }
            else
            {
                // Offscreen position
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
        // Initialize positions and scales
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            GameObject model = instantiatedModels[i];
 
            int relativeIndex = (i - currentIndex + instantiatedModels.Count) % instantiatedModels.Count;
 
            Vector3 targetPosition;
            Vector3 targetScale;
 
            if (relativeIndex == 0)
            {
                // Center position
                targetPosition = centerPosition;
                targetScale = centerScale;
            }
            else if (relativeIndex == 1)
            {
                // Right position
                targetPosition = rightPosition;
                targetScale = sideScale;
            }
            else if (relativeIndex == instantiatedModels.Count - 1)
            {
                // Left position
                targetPosition = leftPosition;
                targetScale = sideScale;
            }
            else
            {
                // Offscreen position
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
        // Update the appearance (e.g., lighting) of the models
        for (int i = 0; i < instantiatedModels.Count; i++)
        {
            Renderer renderer = instantiatedModels[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                int relativeIndex = (i - currentIndex + instantiatedModels.Count) % instantiatedModels.Count;
                Material material = renderer.material; // This creates an instance of the material
                if (relativeIndex == 0)
                {
                    // Center model, reset color to original
                    material.color = originalColors[i];
 
                    // Enable emission if desired
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.white);
                }
                else
                {
                    // Non-center models, darken
                    Color originalColor = originalColors[i];
                    Color darkenedColor = new Color(originalColor.r * darkeningAmount, originalColor.g * darkeningAmount, originalColor.b * darkeningAmount, originalColor.a);
                    material.color = darkenedColor;
 
                    // Disable emission
                    material.DisableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.black);
                }
            }
 
            // Handle lock symbol
            if (lockSymbols[i] != null)
            {
                if (!ownedStatus[i])
                {
                    // Model is not owned, enable lock symbol
                    lockSymbols[i].SetActive(true);
                }
                else
                {
                    // Model is owned, disable lock symbol
                    lockSymbols[i].SetActive(false);
                }
            }
        }
    }
 
    // Function to save ownedStatus to PlayerPrefs
    void SaveOwnedStatus()
    {
        OwnedStatusWrapper wrapper = new OwnedStatusWrapper();
        wrapper.ownedStatus = ownedStatus;
 
        // Serialize to JSON
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("OwnedStatus", json);
        PlayerPrefs.Save();
    }
 
    // Function to load ownedStatus from PlayerPrefs
    void LoadOwnedStatus()
    {
        if (resetPlayerDataOnStart)
        {
            // If reset is requested, initialize ownedStatus
            ownedStatus = new bool[models.Length];
            // Optionally, set the first vehicle as owned by default
            ownedStatus[0] = true; // First vehicle is owned
            for (int i = 1; i < ownedStatus.Length; i++)
            {
                ownedStatus[i] = false;
            }
        }
        else if (PlayerPrefs.HasKey("OwnedStatus"))
        {
            string json = PlayerPrefs.GetString("OwnedStatus");
            OwnedStatusWrapper wrapper = JsonUtility.FromJson<OwnedStatusWrapper>(json);
            ownedStatus = wrapper.ownedStatus;
        }
        else
        {
            // If no saved data, initialize ownedStatus
            ownedStatus = new bool[models.Length];
            // Optionally, set the first vehicle as owned by default
            ownedStatus[0] = true; // First vehicle is owned
            for (int i = 1; i < ownedStatus.Length; i++)
            {
                ownedStatus[i] = false;
            }
        }
    }
 
    // Function to call when a purchase is made
    public void OnPurchase()
    {
        if (scoreManager.coins < SelectedPrice) return;
        scoreManager.AddCoin(-SelectedPrice);
        // Set the current vehicle as owned
        ownedStatus[currentIndex] = true;
        // Save the updated ownedStatus array
        SaveOwnedStatus();
        // Update the model appearances
        UpdateModelAppearances();
        // Optionally save the current vehicle name here
        SaveCurrentVehicleName();
    }
 
    // Function to save the current vehicle name
    void SaveCurrentVehicleName()
    {
        string currentVehicleName = models[currentIndex].name;
        PlayerPrefs.SetString("CurrentVehicleName", currentVehicleName);
        PlayerPrefs.Save();
    }
 
    // Function to load the current vehicle name
    void LoadCurrentVehicleName()
    {
        if (PlayerPrefs.HasKey("CurrentVehicleName"))
        {
            string savedVehicleName = PlayerPrefs.GetString("CurrentVehicleName");
            // Find the index of the vehicle with this name
            int index = Array.FindIndex(models, model => model.name == savedVehicleName);
            if (index >= 0)
            {
                currentIndex = index;
            }
            else
            {
                // If not found, default to index 0
                currentIndex = 0;
            }
        }
        else
        {
            // No saved vehicle name, default to index 0
            currentIndex = 0;
        }
    }
 
    // Function to call when a vehicle is selected
    public void OnSelectVehicle()
    {
        SaveCurrentVehicleName();
    }
}
 
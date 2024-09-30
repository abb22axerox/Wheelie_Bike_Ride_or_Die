using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3
using System.Collections.Generic;
using System;
 
[Serializable]
public class VehicleSettings
{
    [Header("Vehicle Configuration")]
    public string vehicleName;      
    public float baseSpeed = 10.0f;
    public bool canWheelie = true;          
    public float accelerationRate = 0.5f;      
    public float maxSpeed = 20.0f;
 
    [Header("Model Settings")]
    public Vector3 modelRotation = Vector3.zero;
    public Vector3 modelPosition = Vector3.zero;
    public Vector3 modelScale = Vector3.one;    
 
    [Header("Engine Sound Settings")]
    public List<AudioClip> engineSounds;      
    public float maxSpeedSoundValue = 20.0f;   
}
 
public class PlayerController : MonoBehaviour
{
    public Camera camera;
    public float a = 1;
    public float b = 1;
    [Header("All Vehicles Settings")]
    public VehicleSettings[] allVehiclesSettings;    
 
    [Header("Current Vehicle Settings")]
    public VehicleSettings currentVehicle;        
 
    [Header("Audio Settings")]
    [NonSerialized] public AudioSource engineAudioSource;  
    public AudioClip crashSound;                  
 
    [Header("Death Sounds")]
    public List<AudioClip> deathSounds;      
 
    [Header("Power-Up Sound Settings")]
    public AudioClip speedUpSound;
    public AudioClip slowDownSound;
    public AudioClip rocketSound;
    private AudioSource rocketAudioSource;      
 
    [Header("Truck Lane Sound")]
    public AudioClip truckLaneSound;
    private AudioSource truckLaneAudioSource;
    private bool isNearTruckLane = false;
 
    [Header("Movement Settings")]
    public float laneChangeCooldown = 0.5f;
    public float laneChangeDuration = 0.5f;
 
    [Header("Wheelie Settings")]
    public float startInvulnerabilityTime = 5.0f;
    public float maxWheelieAngle = 45.0f;           
    public float wheelieSmoothTime = 0.1f;        
    public bool demandWheelie = false;              
 
    [Header("Tilt Settings")]
    public float maxTiltAngle = 30.0f;         
    public float tiltSmoothTime = 0.1f;        
 
    [Header("Speed-Up Settings")]
    public float speedUpMultiplier = 2.0f;
    public float speedUpDuration = 5.0f;       
    public float speedUpRampUpTime = 1.0f;          
    public float speedUpRampDownTime = 1.0f;  
 
    [Header("Slow-Down Settings")]
    public float slowDownMultiplier = 0.5f;  
    public float slowDownDuration = 5.0f;        
    public float slowDownRampUpTime = 1.0f;  
    public float slowDownRampDownTime = 1.0f;
 
    [Header("Rocket Effect Settings")]
    public float rocketHeight = 5.0f;        
    public float rocketDuration = 5.0f;            
    public float rocketRampUpTime = 1.0f;        
    public float rocketRampDownTime = 1.0f;         
    public float rocketOscillationAmplitude = 0.5f;
    public float rocketOscillationFrequency = 1.0f;
 
    [Header("Spline Settings")]
    public SplineContainer splineContainer;        
 
    [Header("References")]
    public RoadSettings roadSettings;
    public Transform modelPivot;       
    public GameOverScreen gameOverScreen;
    public ScoreManager scoreManager;
    public PowerupBar speedUpBar;
    public PowerupBar slowDownBar;
    public PowerupBar rocketBar;
 
    // New Falling Animation Settings
    [Header("Falling Animation Settings")]
    public float fallOverDuration = 1.0f;  
    public float fallOverAngle = 90.0f;        
    public float spinSpeed = 360.0f;               
    public float fallDownSpeed = 5.0f;              
 
    private int currentLane;                    
    private float targetSideOffset;      
    private float startSideOffset;          
    private bool isChangingLane = false;   
    private float laneChangeTimer = 0.0f;           
    private float cooldownTimer = 0.0f;        
    [NonSerialized] public float SPEED = 0;
 
    private float currentSpeed;                     
    private float targetSpeed;                     
 
    private float currentWheelieAngle = 0.0f;       
    private float targetWheelieAngle = 0.0f;       
    private float wheelieAngleVelocity = 0.0f;    
 
    private float currentTiltAngle = 0.0f;     
    private float targetTiltAngle = 0.0f;      
    private float tiltAngleVelocity = 0.0f;       
 
    private Quaternion initialModelRotation;       
 
    // Speed-Up Effect Variables
    private float speedUpEffectTimer = 0.0f;  
    private bool isSpeedUpActive = false;          
    private float currentSpeedMultiplier = 1.0f;
 
    // Slow-Down Effect Variables
    private float slowDownEffectTimer = 0.0f;       
    private bool isSlowDownActive = false;          
    private float currentSlowMultiplier = 1.0f;   
 
    // Rocket Effect Variables
    private bool isRocketActive = false;         
    private float rocketEffectTimer = 0.0f;      
    private float currentRocketHeight = 0.0f;      
 
    // Falling Over Variables
    private bool isFallingOver = false;    
    private float fallOverTimer = 0.0f;      
 
    // Spline Variables
    private Spline spline;
    public float distanceTraveled = 0f;          
    private float splineLength;          
    private float sideOffset = 0f;          
    private string InputBuffer = "None";
    private float timer = 0;
 
    public string vehicleName;
    private string ForwardButton;
    private string BackwardButton;
    private string LeftButton;
    private string RightButton;
 
    void Start()
    {
        // Load control settings from PlayerPrefs
        ForwardButton = PlayerPrefs.GetString("ForwardButton", "UpArrow");
        BackwardButton = PlayerPrefs.GetString("BackwardButton", "DownArrow");
        LeftButton = PlayerPrefs.GetString("LeftButton", "LeftArrow");
        RightButton = PlayerPrefs.GetString("RightButton", "RightArrow");
 
        // Load the selected vehicle name from PlayerPrefs
        vehicleName = PlayerPrefs.GetString("CurrentVehicleName");
 
        // Get the vehicle settings and model based on the vehicle name
        GetVehicleSettings(vehicleName);
 
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is not assigned in PlayerController.");
            return;
        }
        spline = splineContainer.Spline;
        splineLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
 
        // Initialize the player in the middle lane
        currentLane = roadSettings.numLanes / 2;
        sideOffset = GetLaneSideOffset(currentLane);
        targetSideOffset = sideOffset;
 
        // Initialize speeds based on vehicle settings
        currentSpeed = currentVehicle.baseSpeed;
        targetSpeed = currentSpeed;
 
        if (modelPivot == null)
        {
            Debug.LogError("modelPivot is not assigned in the PlayerController script.");
        }
        else
        {
            // Store the initial rotation of the modelPivot
            initialModelRotation = modelPivot.localRotation;
        }
 
        // Initialize AudioSource for engine sounds
        if (engineAudioSource == null)
        {
            engineAudioSource = gameObject.AddComponent<AudioSource>();
            engineAudioSource.loop = true;
            engineAudioSource.playOnAwake = false;
            engineAudioSource.spatialBlend = 0.0f; // 2D sound
        }
 
        // Start playing the initial engine sound
        PlayEngineSound();
 
        // Initialize AudioSource for rocket sound
        rocketAudioSource = gameObject.AddComponent<AudioSource>();
        rocketAudioSource.loop = true;
        rocketAudioSource.playOnAwake = false;
        rocketAudioSource.spatialBlend = 0.0f; // 2D sound
        rocketAudioSource.clip = rocketSound;
 
        // Initialize AudioSource for truck lane sound
        truckLaneAudioSource = gameObject.AddComponent<AudioSource>();
        truckLaneAudioSource.loop = true;
        truckLaneAudioSource.playOnAwake = false;
        truckLaneAudioSource.spatialBlend = 0.0f; // 2D sound
        truckLaneAudioSource.clip = truckLaneSound;
    }
 
    void Update()
    {
        // If the bike is falling over, handle the fall animation
        if (isFallingOver)
        {
            HandleFallingOver();
            return; // Skip the rest of the update to prevent player control
        }
 
        // Update cooldown timer
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
 
        // Handle input buffer
        if (Input.GetKeyDown(LeftButton)) InputBuffer = "Left";
        else if (Input.GetKeyDown(RightButton)) InputBuffer = "Right";
 
        // Handle lane change input
        if (!isChangingLane && cooldownTimer <= 0)
        {
            if (InputBuffer == "Left")
            {
                MoveLeft();
                InputBuffer = "None";
            }
            else if (InputBuffer == "Right")
            {
                MoveRight();
                InputBuffer = "None";
            }
        }
 
        // Handle lane changing movement
        if (isChangingLane)
        {
            laneChangeTimer += Time.deltaTime;
            float t = laneChangeTimer / laneChangeDuration;
            t = Mathf.Clamp01(t); // Ensure t doesn't go beyond 1
 
            // Smoothly interpolate between the start and target side offsets
            sideOffset = Mathf.Lerp(startSideOffset, targetSideOffset, t);
 
            // Reset lane change when complete
            if (t >= 1.0f)
            {
                isChangingLane = false;
                laneChangeTimer = 0.0f;
                cooldownTimer = laneChangeCooldown;
                targetTiltAngle = 0.0f; // Reset tilt after lane change
            }
        }
 
        // Handle speed-related values
        HandleSpeedUp();
        HandleSlowDown();
        HandleWheelieAndSpeed();
 
        // Increase the distance traveled based on current speed
        distanceTraveled += currentSpeed * (isRocketActive ? 1.5f : 1.0f) * Time.deltaTime;
 
        if (distanceTraveled > splineLength)
        {
            distanceTraveled %= splineLength; // Wrap the travled distance around the spline length (to allow looping around the track)
        }
 
        // Get the position along the spline with side offset
        Vector3 position = SplineUtilityExtension.GetPositionAtDistance(
            distanceTraveled,
            spline,
            splineContainer.transform,
            sideOffset
        );
 
        position = HandleRocketPosition(position);
 
        // Set the player's position
        transform.position = position;
 
        float tRotation = distanceTraveled / splineLength;
        tRotation = Mathf.Repeat(tRotation, 1f); // Ensure t is between 0 and 1
 
        SplineUtility.Evaluate(spline, tRotation, out _, out float3 tangent, out _);
        Vector3 worldTangent = splineContainer.transform.TransformDirection((Vector3)tangent);
 
        if (worldTangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(worldTangent);
        }
 
        // Handle tilt during lane changes
        HandleTilt();
 
        // Apply wheelie and tilt angles to modelPivot rotation
        if (modelPivot != null)
        {
            Quaternion wheelieRotation = Quaternion.Euler(0, 0, -currentWheelieAngle);
            Quaternion tiltRotation = Quaternion.Euler(-currentTiltAngle, 0, 0);
            modelPivot.localRotation = initialModelRotation * wheelieRotation * tiltRotation;
        }
 
        // Update engine sound based on current speed
        UpdateEngineSound();
    }
 
    void GetVehicleSettings(string vehicleName)
    {
        bool vehicleFound = false;
 
        foreach (VehicleSettings vs in allVehiclesSettings)
        {
            if (vs.vehicleName == vehicleName)
            {
                currentVehicle = vs;
                vehicleFound = true;
                break;
            }
        }
 
        if (!vehicleFound)
        {
            Debug.LogWarning("Vehicle with name '" + vehicleName + "' not found. Using default settings.");
            if (allVehiclesSettings.Length > 0)
            {
                currentVehicle = allVehiclesSettings[0]; // Use the first vehicle as default
            }
            else
            {
                Debug.LogError("No vehicle settings available in 'allVehiclesSettings'.");
            }
        }
 
        // Delete previous vehicle model(s)
        if (modelPivot.childCount > 0)
        {
            foreach (Transform child in modelPivot)
            {
                Destroy(child.gameObject);
            }
        }
 
        // Load new vehicle model
        GameObject vehiclePrefab = Resources.Load<GameObject>("Prefabs/PlayerModelPrefabs/" + vehicleName);
 
        if (vehiclePrefab != null)
        {
            GameObject newVehicle = Instantiate(vehiclePrefab, modelPivot);
 
            newVehicle.transform.SetLocalPositionAndRotation(currentVehicle.modelPosition, Quaternion.Euler(currentVehicle.modelRotation));
            newVehicle.transform.localScale = currentVehicle.modelScale;
 
            Debug.Log(vehicleName + " has been loaded");
        }
        else
        {
            Debug.LogError("Vehicle prefab not found for vehicle name: " + vehicleName);
        }
    }
 
    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            targetTiltAngle = maxTiltAngle; // Tilt to the left
            StartLaneChange();
        }
    }
 
    void MoveRight()
    {
        if (currentLane < roadSettings.numLanes - 1)
        {
            currentLane++;
            targetTiltAngle = -maxTiltAngle; // Tilt to the right
            StartLaneChange();
        }
    }
 
    void StartLaneChange()
    {
        startSideOffset = sideOffset;
        targetSideOffset = GetLaneSideOffset(currentLane);
        isChangingLane = true;
        laneChangeTimer = 0.0f;
    }
 
    float GetLaneSideOffset(int laneIndex)
    {
        // Calculate side offset based on lane index
        float centerLane = (roadSettings.numLanes - 1) / 2.0f;
        return (laneIndex - centerLane) * roadSettings.laneWidth;
    }
 
    Vector3 HandleRocketPosition(Vector3 position)
    {
        if (isRocketActive)
        {
            rocketEffectTimer += Time.deltaTime;
 
            float totalDuration = rocketRampUpTime + rocketDuration + rocketRampDownTime;
 
            if (rocketEffectTimer < rocketRampUpTime)
            {
                // Ascending phase
                float t = rocketEffectTimer / rocketRampUpTime;
                currentRocketHeight = Mathf.Lerp(0.0f, rocketHeight, t);
            }
            else if (rocketEffectTimer < rocketRampUpTime + rocketDuration)
            {
                // Full height phase
                currentRocketHeight = rocketHeight;
            }
            else if (rocketEffectTimer < totalDuration)
            {
                // Descending phase
                float t = (rocketEffectTimer - rocketRampUpTime - rocketDuration) / rocketRampDownTime;
                currentRocketHeight = Mathf.Lerp(rocketHeight, 0.0f, t);
            }
            else
            {
                // Rocket effect has ended
                currentRocketHeight = 0.0f;
                isRocketActive = false;
 
                // Stop the rocket sound
                if (rocketAudioSource != null && rocketAudioSource.isPlaying)
                {
                    rocketAudioSource.Stop();
                }
            }
 
            // Use sin() to make the player bob up and down
            float oscillation = 0.0f;
            if (currentRocketHeight > 0.0f)
            {
                oscillation = rocketOscillationAmplitude * Mathf.Sin(rocketOscillationFrequency * rocketEffectTimer * Mathf.PI * 2);
            }
 
            // Update the player's Y position by adding to the spline's Y position
            position.y += currentRocketHeight + oscillation;
        }
 
        return position;
    }
 
    void CheckWheelieAngle(float wheelieAngle)
    {
        if (!currentVehicle.canWheelie || !demandWheelie) return;
 
        const float errorTol = 5f;
        bool doKillPlayer = wheelieAngle > maxWheelieAngle - errorTol || wheelieAngle <= errorTol;
        if (doKillPlayer)
        {
            Debug.Log("Killed by failing to wheelie");
            StartFallingOver();
        }
    }
 
    void HandleWheelieAndSpeed()
    {
        // Determine acceleration input based on key presses
        float accelerationInput = 0.0f;
        if (Input.GetKey(ForwardButton))
        {
            accelerationInput = 1.0f;
        }
        else if (Input.GetKey(BackwardButton))
        {
            accelerationInput = -1.0f;
        }
 
        // Handle wheelie if vehicle is allowed to wheelie
        if (currentVehicle.canWheelie)
        {
            if (accelerationInput > 0)
            {
                targetWheelieAngle = maxWheelieAngle;
            }
            else
            {
                targetWheelieAngle = 0.0f;
            }
 
            currentWheelieAngle = Mathf.SmoothDamp(currentWheelieAngle, targetWheelieAngle, ref wheelieAngleVelocity, wheelieSmoothTime);
 
            timer += Time.deltaTime;
            if (timer > startInvulnerabilityTime) CheckWheelieAngle(currentWheelieAngle);
 
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0.0f, maxWheelieAngle);
 
            float wheelieFactor = currentWheelieAngle / maxWheelieAngle;
 
            float speedBoost = wheelieFactor * (currentVehicle.maxSpeed - currentVehicle.baseSpeed);
 
            targetSpeed = currentVehicle.baseSpeed + speedBoost;
        }
        else
        {
            if (accelerationInput > 0)
            {
                // Accelerate towards max speed
                targetSpeed = currentVehicle.maxSpeed;
            }
            else if (accelerationInput < 0)
            {
                // Decelerate towards a minimum speed
                targetSpeed = currentVehicle.baseSpeed;
            }
            else
            {
                // Maintain current speed
                targetSpeed = currentSpeed;
            }
 
            // Ensure wheelie angle is zero
            currentWheelieAngle = 0.0f;
            targetWheelieAngle = 0.0f;
        }
 
        float adjustedTargetSpeed = targetSpeed * currentSpeedMultiplier * currentSlowMultiplier;
 
        // Smoothly adjust current speed towards adjusted target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, adjustedTargetSpeed, currentVehicle.accelerationRate * Time.deltaTime);
        SPEED = currentSpeed;
        if (!(demandWheelie && currentVehicle.canWheelie)) camera.fieldOfView = SPEED * a + b;
 
        // Clamp current speed
        float minSpeed = currentVehicle.baseSpeed * currentSpeedMultiplier * currentSlowMultiplier;
        float maxSpeed = currentVehicle.maxSpeed * currentSpeedMultiplier * currentSlowMultiplier;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }
 
    void HandleTilt()
    {
        if (!isChangingLane) targetTiltAngle = 0.0f;
 
        // Smoothly adjust current tilt angle towards target tilt angle
        currentTiltAngle = Mathf.SmoothDamp(currentTiltAngle, targetTiltAngle, ref tiltAngleVelocity, tiltSmoothTime);
 
        // Clamp tilt angle
        currentTiltAngle = Mathf.Clamp(currentTiltAngle, -maxTiltAngle, maxTiltAngle);
    }
 
    void HandleSpeedUp()
    {
        if (isSpeedUpActive)
        {
            // Increment the speed-up effect timer
            speedUpEffectTimer += Time.deltaTime;
 
            float totalDuration = speedUpRampUpTime + speedUpDuration + speedUpRampDownTime;
 
            if (speedUpEffectTimer < speedUpRampUpTime)
            {
                // Ramp up phase
                float t = speedUpEffectTimer / speedUpRampUpTime;
                currentSpeedMultiplier = Mathf.Lerp(1.0f, speedUpMultiplier, t);
            }
            else if (speedUpEffectTimer < speedUpRampUpTime + speedUpDuration)
            {
                // Full speed-up phase
                currentSpeedMultiplier = speedUpMultiplier;
            }
            else if (speedUpEffectTimer < totalDuration)
            {
                // Ramp down phase
                float t = (speedUpEffectTimer - speedUpRampUpTime - speedUpDuration) / speedUpRampDownTime;
                currentSpeedMultiplier = Mathf.Lerp(speedUpMultiplier, 1.0f, t);
            }
            else
            {
                // Speed-up effect has ended
                currentSpeedMultiplier = 1.0f;
                isSpeedUpActive = false;
            }
        }
        else
        {
            currentSpeedMultiplier = 1.0f;
        }
    }
 
    void HandleSlowDown()
    {
        if (isSlowDownActive)
        {
            // Increment the slow-down effect timer
            slowDownEffectTimer += Time.deltaTime;
 
            float totalDuration = slowDownRampUpTime + slowDownDuration + slowDownRampDownTime;
 
            if (slowDownEffectTimer < slowDownRampUpTime)
            {
                float t = slowDownEffectTimer / slowDownRampUpTime;
                currentSlowMultiplier = Mathf.Lerp(1.0f, slowDownMultiplier, t);
            }
            else if (slowDownEffectTimer < slowDownRampUpTime + slowDownDuration)
            {
                // Full slow-down phase
                currentSlowMultiplier = slowDownMultiplier;
            }
            else if (slowDownEffectTimer < totalDuration)
            {
                float t = (slowDownEffectTimer - slowDownRampUpTime - slowDownDuration) / slowDownRampDownTime;
                currentSlowMultiplier = Mathf.Lerp(slowDownMultiplier, 1.0f, t);
            }
            else
            {
                // Slow-down effect has ended
                currentSlowMultiplier = 1.0f;
                isSlowDownActive = false;
            }
        }
        else
        {
            currentSlowMultiplier = 1.0f;
        }
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Killed by crashing into an obstacle");
            StartFallingOver();
        }
    }
 
    void StartFallingOver()
    {
        isFallingOver = true;
        fallOverTimer = 0.0f;
 
        // Stop engine sound
        if (engineAudioSource != null && engineAudioSource.isPlaying)
        {
            engineAudioSource.Stop();
        }
 
        // Stop any power-up sounds
        if (rocketAudioSource != null && rocketAudioSource.isPlaying)
        {
            rocketAudioSource.Stop();
        }
        if (truckLaneAudioSource != null && truckLaneAudioSource.isPlaying)
        {
            truckLaneAudioSource.Stop();
        }
 
        AudioSource.PlayClipAtPoint(crashSound, transform.position);
    }
 
    void HandleFallingOver()
    {
        fallOverTimer += Time.deltaTime;
        float t = fallOverTimer / fallOverDuration;
 
        if (modelPivot != null)
        {
            float fallAngle = Mathf.Lerp(0.0f, fallOverAngle, t);
 
            float spinAngle = spinSpeed * Time.deltaTime;
 
            modelPivot.localRotation = initialModelRotation * Quaternion.Euler(fallAngle, 0, 0);
            modelPivot.Rotate(0, spinAngle, 0, Space.Self);
        }
 
        if (t >= 1.0f)
        {
            isFallingOver = false;
            Time.timeScale = 0f;
 
            gameOverScreen.Setup(scoreManager.score);
        }
    }
 
    // -------------------- Power-Up Handling --------------------
 
    public void DoSpeedUp()
    {
        // Play speed-up sound
        if (speedUpSound != null)
        {
            AudioSource.PlayClipAtPoint(speedUpSound, transform.position);
        }
 
        // Apply speed up temporarily
        isSpeedUpActive = true;
        speedUpEffectTimer = 0.0f;
 
        speedUpBar.powerupSlider.value = 1f;
        speedUpBar.StartDraining();
        speedUpBar.powerupSlider.gameObject.SetActive(true);
    }
 
    public void DoSlowDown()
    {
        // Play slow-down sound
        if (slowDownSound != null)
        {
            AudioSource.PlayClipAtPoint(slowDownSound, transform.position);
        }
 
        // Apply slow down temporarily
        isSlowDownActive = true;
        slowDownEffectTimer = 0.0f;
 
        slowDownBar.powerupSlider.value = 1f;
        slowDownBar.StartDraining();
        slowDownBar.powerupSlider.gameObject.SetActive(true);
    }
 
    public void DoRocket()
    {
        // Activate the rocket effect
        isRocketActive = true;
        rocketEffectTimer = 0.0f;
 
        // Play rocket sound
        if (rocketAudioSource != null && rocketSound != null)
        {
            rocketAudioSource.Play();
        }
 
        rocketBar.powerupSlider.value = 1f;
        rocketBar.StartDraining();
        rocketBar.powerupSlider.gameObject.SetActive(true);
    }
 
    // -------------------- Engine Sound Handling --------------------
 
    void PlayEngineSound()
    {
        if (currentVehicle.engineSounds == null || currentVehicle.engineSounds.Count == 0)
        {
            Debug.LogWarning("No engine sounds assigned in VehicleSettings.");
            return;
        }
 
        // Select the initial engine sound based on current speed
        int soundIndex = GetEngineSoundIndex(currentSpeed);
        if (soundIndex >= 0 && soundIndex < currentVehicle.engineSounds.Count)
        {
            engineAudioSource.clip = currentVehicle.engineSounds[soundIndex];
            engineAudioSource.volume = CalculateEngineVolume(currentSpeed);
            engineAudioSource.Play();
        }
    }
 
    void UpdateEngineSound()
    {
        if (currentVehicle.engineSounds == null || currentVehicle.engineSounds.Count == 0)
        {
            return;
        }
 
        int desiredSoundIndex = GetEngineSoundIndex(currentSpeed);
 
        if (desiredSoundIndex < 0 || desiredSoundIndex >= currentVehicle.engineSounds.Count)
        {
            return;
        }
 
        if (engineAudioSource.clip != currentVehicle.engineSounds[desiredSoundIndex])
        {
            engineAudioSource.clip = currentVehicle.engineSounds[desiredSoundIndex];
            engineAudioSource.volume = CalculateEngineVolume(currentSpeed);
            engineAudioSource.Play();
        }
        else
        {
            engineAudioSource.volume = CalculateEngineVolume(currentSpeed);
        }
    }
 
    int GetEngineSoundIndex(float speed)
    {
        if (currentVehicle.engineSounds == null || currentVehicle.engineSounds.Count == 0)
            return -1;
 
        float speedRatio = Mathf.Clamp(speed / currentVehicle.maxSpeedSoundValue, 0f, 1f);
        int totalSounds = currentVehicle.engineSounds.Count;
        int index = Mathf.FloorToInt(speedRatio * (totalSounds));
 
        // Clamp index to valid range
        index = Mathf.Clamp(index, 0, totalSounds - 1);
        return index;
    }
 
    float CalculateEngineVolume(float speed)
    {
        float speedRatio = Mathf.Clamp(speed / currentVehicle.maxSpeedSoundValue, 0f, 1f);
        return speedRatio; // Volume ranges from 0 to 1 based on speed ratio
    }
 
    // -------------------- End of Engine Sound Handling --------------------
}
 
 
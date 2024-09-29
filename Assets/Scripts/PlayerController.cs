using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3
using System.Collections.Generic;
 
[System.Serializable]
public class VehicleSettings
{
    [Header("Vehicle Configuration")]
    public string vehicleName;                   // Name of the vehicle
    public float baseSpeed = 10.0f;              // Base forward speed
    public bool canWheelie = true;               // Ability to perform wheelies
    public float accelerationRate = 0.5f;        // How quickly the vehicle can accelerate
    public float maxSpeed = 20.0f;               // Maximum speed the vehicle can achieve
 
    [Header("Model Settings")]
    public Vector3 modelRotation = Vector3.zero; // Rotation to apply to the vehicle model
    public Vector3 modelPosition = Vector3.zero; // Position offset for the vehicle model
    public Vector3 modelScale = Vector3.one;     // Scale for the vehicle model
 
    [Header("Engine Sound Settings")]
    public List<AudioClip> engineSounds;         // Engine sounds ordered from quiet to loud
    public float maxSpeedSoundValue = 20.0f;     // Speed at which the loudest sound plays at full volume
}
 
public class PlayerController : MonoBehaviour
{
    [Header("All Vehicles Settings")]
    public VehicleSettings[] allVehiclesSettings;    // Array to hold multiple vehicle settings
 
    [Header("Current Vehicle Settings")]
    public VehicleSettings currentVehicle;           // Current vehicle's settings
 
    [Header("Audio Settings")]
    public AudioSource engineAudioSource;            // AudioSource for engine sounds
    public AudioClip crashSound;                     // Crash sound effect assigned through the Inspector
 
    [Header("Movement Settings")]
    public float laneChangeCooldown = 0.5f;         // Cooldown time between lane changes
    public float laneChangeDuration = 0.5f;         // Time it takes to move to a new lane
 
    [Header("Wheelie Settings")]
    public float startInvulnerabilityTime = 5.0f;
    public float maxWheelieAngle = 45.0f;           // Maximum wheelie angle in degrees
    public float wheelieSmoothTime = 0.1f;          // Time it takes to smooth the wheelie angle
    public bool demandWheelie = false;              // Require wheelie within tolerance
 
    [Header("Tilt Settings")]
    public float maxTiltAngle = 30.0f;              // Maximum tilt angle when changing lanes
    public float tiltSmoothTime = 0.1f;             // Time it takes to smooth the tilt angle
 
    [Header("Speed-Up Settings")]
    public float speedUpMultiplier = 2.0f;          // Multiplier for the speed-up effect
    public float speedUpDuration = 5.0f;            // Duration of the speed-up effect at full strength
    public float speedUpRampUpTime = 1.0f;          // Time to ramp up to full speed-up
    public float speedUpRampDownTime = 1.0f;        // Time to ramp down from full speed-up
 
    [Header("Slow-Down Settings")]
    public float slowDownMultiplier = 0.5f;         // Multiplier for the slow-down effect
    public float slowDownDuration = 5.0f;           // Duration of the slow-down effect at full strength
    public float slowDownRampUpTime = 1.0f;         // Time to ramp down to full slow-down
    public float slowDownRampDownTime = 1.0f;       // Time to ramp up from full slow-down
 
    [Header("Rocket Effect Settings")]
    public float rocketHeight = 5.0f;               // Height to which the player moves up during the rocket effect
    public float rocketDuration = 5.0f;             // Duration of the rocket effect at full height
    public float rocketRampUpTime = 1.0f;           // Time to ascend to full height
    public float rocketRampDownTime = 1.0f;         // Time to descend back to original height
    public float rocketOscillationAmplitude = 0.5f; // Amplitude of the vertical oscillation
    public float rocketOscillationFrequency = 1.0f; // Frequency of the vertical oscillation
 
    [Header("Spline Settings")]
    public SplineContainer splineContainer;         // Reference to the SplineContainer
 
    [Header("References")]
    public RoadSettings roadSettings;
    public Transform modelPivot;                    // Reference to the modelPivot child object
    public GameOverScreen gameOverScreen;
    public ScoreManager scoreManager;
 
    // New Falling Animation Settings
    [Header("Falling Animation Settings")]
    public float fallOverDuration = 1.0f;           // Duration of the falling over animation
    public float fallOverAngle = 90.0f;             // Angle to rotate around X-axis when falling
    public float spinSpeed = 360.0f;                // Spin speed around Y-axis in degrees per second
    public float fallDownSpeed = 5.0f;              // Speed at which the bike falls down along Y-axis
 
    private int currentLane;                        // Current lane index (0 is the leftmost lane)
    private float targetSideOffset;                 // Target side offset after lane change
    private float startSideOffset;                  // Starting side offset before lane change
    private bool isChangingLane = false;            // Is the player currently changing lanes
    private float laneChangeTimer = 0.0f;           // Timer for lane change interpolation
    private float cooldownTimer = 0.0f;             // Timer for lane change cooldown
 
    private float currentSpeed;                     // Current forward speed
    private float targetSpeed;                      // Target forward speed
 
    private float currentWheelieAngle = 0.0f;       // Current wheelie angle
    private float targetWheelieAngle = 0.0f;        // Target wheelie angle
    private float wheelieAngleVelocity = 0.0f;      // Velocity reference for SmoothDamp
 
    private float currentTiltAngle = 0.0f;          // Current tilt angle
    private float targetTiltAngle = 0.0f;           // Target tilt angle
    private float tiltAngleVelocity = 0.0f;         // Velocity reference for SmoothDamp
 
    private Quaternion initialModelRotation;        // Initial rotation of the modelPivot
 
    // Speed-Up Effect Variables
    private float speedUpEffectTimer = 0.0f;        // Timer for the speed-up effect
    private bool isSpeedUpActive = false;           // Is the speed-up effect active
    private float currentSpeedMultiplier = 1.0f;    // Current speed-up multiplier
 
    // Slow-Down Effect Variables
    private float slowDownEffectTimer = 0.0f;       // Timer for the slow-down effect
    private bool isSlowDownActive = false;          // Is the slow-down effect active
    private float currentSlowMultiplier = 1.0f;     // Current slow-down multiplier
 
    // Rocket Effect Variables
    private bool isRocketActive = false;            // Is the rocket effect active
    private float rocketEffectTimer = 0.0f;         // Timer for the rocket effect
    private float currentRocketHeight = 0.0f;       // Current height offset due to rocket effect
 
    // Falling Over Variables
    private bool isFallingOver = false;             // Is the bike currently falling over
    private float fallOverTimer = 0.0f;             // Timer for the falling over animation
 
    // Spline Variables
    private Spline spline;
    public float distanceTraveled = 0f;             // Total distance traveled along the spline
    private float splineLength;                     // Total length of the spline
    private float sideOffset = 0f;                  // Current side offset in the XZ-plane
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
 
        // Ensure the spline container is assigned
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is not assigned in PlayerController.");
            return;
        }
 
        // Get the spline from the container
        spline = splineContainer.Spline;
 
        // Calculate the total length of the spline in world space
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
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
 
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
 
        // Handle speed-up and slow-down effects
        HandleSpeedUp();
        HandleSlowDown();
 
        // Handle wheelie input and speed adjustments
        HandleWheelieAndSpeed();
 
        // Increase the distance traveled based on current speed
        distanceTraveled += currentSpeed * (isRocketActive ? 2 : 1) * Time.deltaTime;
 
        // Handle looping or clamping at the end of the spline
        if (distanceTraveled > splineLength)
        {
            // For now, loop back to start
            distanceTraveled %= splineLength; // Loop back to the start
        }
 
        // Get the position along the spline with side offset
        Vector3 position = SplineUtilityExtension.GetPositionAtDistance(
            distanceTraveled,
            spline,
            splineContainer.transform,
            sideOffset
        );
 
        // Handle rocket effect (modify Y position)
        position = HandleRocketPosition(position);
 
        // Set the player's position
        transform.position = position;
 
        // Optionally, set the player's rotation to face along the spline
        // Evaluate the tangent at the current distance
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
 
            // Apply model settings from VehicleSettings
            newVehicle.transform.localPosition = currentVehicle.modelPosition;
            newVehicle.transform.localRotation = Quaternion.Euler(currentVehicle.modelRotation);
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
            // Increment the rocket effect timer
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
            }
 
            // Apply vertical oscillation during the rocket effect
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
 
            // Play the crash sound effect
            if (crashSound != null)
            {
                AudioSource.PlayClipAtPoint(crashSound, transform.position);
            }
            else
            {
                Debug.LogWarning("Crash sound not assigned in the Inspector.");
            }
        }
    }
 
    void HandleWheelieAndSpeed()
    {
        // Determine acceleration input based on key presses
        float accelerationInput = 0.0f;
 
        if (Input.GetKey(ForwardButton))
        {
            accelerationInput = 1.0f; // Accelerate
        }
        else if (Input.GetKey(BackwardButton))
        {
            accelerationInput = -1.0f; // Decelerate
        }
 
        // Handle wheelie if allowed
        if (currentVehicle.canWheelie)
        {
            if (accelerationInput > 0)
            {
                // Increase wheelie angle
                targetWheelieAngle = maxWheelieAngle;
            }
            else
            {
                // Return to level wheelie angle
                targetWheelieAngle = 0.0f;
            }
 
            // Smoothly adjust wheelie angle towards target angle
            currentWheelieAngle = Mathf.SmoothDamp(currentWheelieAngle, targetWheelieAngle, ref wheelieAngleVelocity, wheelieSmoothTime);
 
            timer += Time.deltaTime;
            if (timer > startInvulnerabilityTime) CheckWheelieAngle(currentWheelieAngle);
 
            // Clamp wheelie angle between 0 and maxWheelieAngle
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0.0f, maxWheelieAngle);
 
            // Compute wheelie factor (0 to 1)
            float wheelieFactor = currentWheelieAngle / maxWheelieAngle;
 
            // Compute speed boost based on wheelie angle
            float speedBoost = wheelieFactor * (currentVehicle.maxSpeed - currentVehicle.baseSpeed);
 
            // Set target speed based on base speed and speed boost
            targetSpeed = currentVehicle.baseSpeed + speedBoost;
        }
        else
        {
            // When wheelie is disabled, adjust target speed based on acceleration input
            if (accelerationInput > 0)
            {
                // Accelerate towards max speed
                targetSpeed = currentVehicle.maxSpeed;
            }
            else if (accelerationInput < 0)
            {
                // Decelerate towards a minimum speed
                targetSpeed = currentVehicle.baseSpeed; // Or define a minimum speed
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
 
        // Apply speed-up and slow-down multipliers to targetSpeed
        float adjustedTargetSpeed = targetSpeed * currentSpeedMultiplier * currentSlowMultiplier;
 
        // Smoothly adjust current speed towards adjusted target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, adjustedTargetSpeed, currentVehicle.accelerationRate * Time.deltaTime);
 
        // Clamp current speed
        float minSpeed = currentVehicle.baseSpeed * currentSpeedMultiplier * currentSlowMultiplier;
        float maxSpeed = currentVehicle.maxSpeed * currentSpeedMultiplier * currentSlowMultiplier;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }
 
    void HandleTilt()
    {
        // If lane change is complete, reset target tilt angle
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
                // Ramp down phase (decrease speed)
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
                // Ramp up phase (return to normal speed)
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
 
            // Play the crash sound effect
            if (crashSound != null)
            {
                AudioSource.PlayClipAtPoint(crashSound, transform.position);
            }
            else
            {
                Debug.LogWarning("Crash sound not assigned in the Inspector.");
            }
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
    }
 
    void HandleFallingOver()
    {
        fallOverTimer += Time.deltaTime;
        float t = fallOverTimer / fallOverDuration;
 
        // Rotate the bike to simulate falling over while spinning around Y-axis
        if (modelPivot != null)
        {
            // Rotate around X-axis to fall over
            float fallAngle = Mathf.Lerp(0.0f, fallOverAngle, t);
 
            // Spin around Y-axis
            float spinAngle = spinSpeed * Time.deltaTime;
 
            // Apply rotations and translations
            modelPivot.localRotation = initialModelRotation * Quaternion.Euler(fallAngle, 0, 0);
            modelPivot.Rotate(0, spinAngle, 0, Space.Self);
        }
 
        if (t >= 1.0f)
        {
            // Falling animation completed
            isFallingOver = false;
            Time.timeScale = 0f;
 
            gameOverScreen.Setup(scoreManager.score);
        }
    }
 
    public void DoSpeedUp()
    {
        // Apply speed up temporarily
        isSpeedUpActive = true;
        speedUpEffectTimer = 0.0f;
    }
 
    public void DoSlowDown()
    {
        // Apply slow down temporarily
        isSlowDownActive = true;
        slowDownEffectTimer = 0.0f;
    }
 
    public void DoRocket()
    {
        // Activate the rocket effect
        isRocketActive = true;
        rocketEffectTimer = 0.0f;
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
 
        // If the desired sound is different from the current, switch it
        if (engineAudioSource.clip != currentVehicle.engineSounds[desiredSoundIndex])
        {
            engineAudioSource.clip = currentVehicle.engineSounds[desiredSoundIndex];
            engineAudioSource.volume = CalculateEngineVolume(currentSpeed);
            engineAudioSource.Play();
        }
        else
        {
            // Adjust volume based on current speed
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
 
 
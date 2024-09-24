using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float laneChangeCooldown = 0.5f;    // Cooldown time between lane changes
    public float laneChangeDuration = 0.5f;    // Time it takes to move to a new lane

    [Header("Speed Settings")]
    public float baseSpeed = 10.0f;            // Base forward speed
    public float maxSpeedBoost = 10.0f;        // Maximum additional speed during wheelie
    public float speedAcceleration = 0.5f;     // Time it takes to reach target speed

    [Header("Wheelie Settings")]
    public float maxWheelieAngle = 45.0f;      // Maximum wheelie angle in degrees
    public float wheelieSmoothTime = 0.1f;     // Time it takes to smooth the wheelie angle
    public bool demandWheelie = false;         // Require wheelie within tolerance

    [Header("Tilt Settings")]
    public float maxTiltAngle = 30.0f;         // Maximum tilt angle when changing lanes
    public float tiltSmoothTime = 0.1f;        // Time it takes to smooth the tilt angle

    [Header("Speed-Up Settings")]
    public float speedUpMultiplier = 2.0f;     // Multiplier for the speed-up effect
    public float speedUpDuration = 5.0f;       // Duration of the speed-up effect at full strength
    public float speedUpRampUpTime = 1.0f;     // Time to ramp up to full speed-up
    public float speedUpRampDownTime = 1.0f;   // Time to ramp down from full speed-up

    [Header("Slow-Down Settings")]
    public float slowDownMultiplier = 0.5f;     // Multiplier for the slow-down effect
    public float slowDownDuration = 5.0f;       // Duration of the slow-down effect at full strength
    public float slowDownRampUpTime = 1.0f;     // Time to ramp down to full slow-down
    public float slowDownRampDownTime = 1.0f;   // Time to ramp up from full slow-down

    [Header("Rocket Effect Settings")]
    public float rocketHeight = 5.0f;              // Height to which the player moves up during the rocket effect
    public float rocketDuration = 5.0f;            // Duration of the rocket effect at full height
    public float rocketRampUpTime = 1.0f;          // Time to ascend to full height
    public float rocketRampDownTime = 1.0f;        // Time to descend back to original height
    public float rocketOscillationAmplitude = 0.5f; // Amplitude of the vertical oscillation
    public float rocketOscillationFrequency = 1.0f; // Frequency of the vertical oscillation

    [Header("Spline Settings")]
    public SplineContainer splineContainer;        // Reference to the SplineContainer

    [Header("References")]
    public RoadSettings roadSettings;
    public Transform modelPivot;                   // Reference to the modelPivot child object

    private int currentLane;                       // Current lane index (0 is the leftmost lane)
    private float targetSideOffset;                // Target side offset after lane change
    private float startSideOffset;                 // Starting side offset before lane change
    private bool isChangingLane = false;           // Is the player currently changing lanes
    private float laneChangeTimer = 0.0f;          // Timer for lane change interpolation
    private float cooldownTimer = 0.0f;            // Timer for lane change cooldown

    private float currentSpeed;                    // Current forward speed
    private float targetSpeed;                     // Target forward speed

    private float currentWheelieAngle = 0.0f;      // Current wheelie angle
    private float targetWheelieAngle = 0.0f;       // Target wheelie angle
    private float wheelieAngleVelocity = 0.0f;     // Velocity reference for SmoothDamp

    private float currentTiltAngle = 0.0f;         // Current tilt angle
    private float targetTiltAngle = 0.0f;          // Target tilt angle
    private float tiltAngleVelocity = 0.0f;        // Velocity reference for SmoothDamp

    private Quaternion initialModelRotation;       // Initial rotation of the modelPivot

    // Speed-Up Effect Variables
    private float speedUpEffectTimer = 0.0f;       // Timer for the speed-up effect
    private bool isSpeedUpActive = false;          // Is the speed-up effect active
    private float currentSpeedMultiplier = 1.0f;   // Current speed-up multiplier

    // Slow-Down Effect Variables
    private float slowDownEffectTimer = 0.0f;      // Timer for the slow-down effect
    private bool isSlowDownActive = false;         // Is the slow-down effect active
    private float currentSlowMultiplier = 1.0f;    // Current slow-down multiplier

    // Rocket Effect Variables
    private bool isRocketActive = false;           // Is the rocket effect active
    private float rocketEffectTimer = 0.0f;        // Timer for the rocket effect
    private float currentRocketHeight = 0.0f;      // Current height offset due to rocket effect

    // Spline Variables
    private Spline spline;
    public float distanceTraveled = 0f;           // Total distance traveled along the spline
    private float splineLength;                    // Total length of the spline
    private float sideOffset = 0f;                 // Current side offset in the XZ-plane
    private string InputBuffer = "None";

    void Start()
    {
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

        // Initialize speeds
        currentSpeed = baseSpeed;
        targetSpeed = baseSpeed;

        if (modelPivot == null)
        {
            Debug.LogError("modelPivot is not assigned in the PlayerController script.");
        }
        else
        {
            // Store the initial rotation of the modelPivot
            initialModelRotation = modelPivot.localRotation;
        }
    }

    void Update()
    {
        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.A)) InputBuffer = "Left";
        else if (Input.GetKeyDown(KeyCode.D)) InputBuffer = "Right";

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
            }
        }

        // Handle speed-up and slow-down effects
        HandleSpeedUp();
        HandleSlowDown();

        // Handle wheelie input and speed adjustments
        HandleWheelieAndSpeed();

        // Increase the distance traveled based on current speed
        distanceTraveled += currentSpeed * Time.deltaTime;

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
        if (!demandWheelie) return;
        
        const float errorTol = 5f;
        bool doKillPlayer = wheelieAngle > maxWheelieAngle - errorTol || wheelieAngle <= errorTol;
        if (doKillPlayer) Debug.Log("Killed by failing the wheelie");
    }

    void HandleWheelieAndSpeed()
    {
        // Determine target wheelie angle based on input
        if (Input.GetKey(KeyCode.W))
        {
            // Increase wheelie angle
            targetWheelieAngle = maxWheelieAngle;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Decrease wheelie angle (lean forward)
            targetWheelieAngle = -maxWheelieAngle;
        }
        else
        {
            // Return to level wheelie angle
            targetWheelieAngle = 0.0f;
        }

        // Smoothly adjust wheelie angle towards target angle
        currentWheelieAngle = Mathf.SmoothDamp(currentWheelieAngle, targetWheelieAngle, ref wheelieAngleVelocity, wheelieSmoothTime);

        // Clamp wheelie angle
        CheckWheelieAngle(currentWheelieAngle);

        currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0, maxWheelieAngle);

        // Compute wheelie factor (-1 to 1)
        float wheelieFactor = currentWheelieAngle / maxWheelieAngle;

        // Compute speed boost based on wheelie angle
        float speedBoost = wheelieFactor * maxSpeedBoost;

        // Set target speed
        targetSpeed = baseSpeed + speedBoost;

        // Apply speed-up and slow-down multipliers to targetSpeed
        float adjustedTargetSpeed = targetSpeed * currentSpeedMultiplier * currentSlowMultiplier;

        // Smoothly adjust current speed towards adjusted target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, adjustedTargetSpeed, speedAcceleration * Time.deltaTime);

        // Clamp current speed
        float minSpeed = (baseSpeed - maxSpeedBoost) * currentSpeedMultiplier * currentSlowMultiplier;
        float maxSpeed = (baseSpeed + maxSpeedBoost) * currentSpeedMultiplier * currentSlowMultiplier;
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
}
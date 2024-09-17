

using UnityEngine;

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
    public bool demandWheelie = true;

    [Header("Tilt Settings")]
    public float maxTiltAngle = 30.0f;         // Maximum tilt angle when changing lanes
    public float tiltSmoothTime = 0.1f;        // Time it takes to smooth the tilt angle

    [Header("References")]
    public RoadSettings roadSettings;
    public Transform modelPivot;               // Reference to the modelPivot child object

    private int currentLane;                   // Current lane index (0 is the leftmost lane)
    private float targetXPosition;             // Target X position after lane change
    private float startXPosition;              // Starting X position before lane change
    private bool isChangingLane = false;       // Is the player currently changing lanes
    private float laneChangeTimer = 0.0f;      // Timer for lane change interpolation
    private float cooldownTimer = 0.0f;        // Timer for lane change cooldown

    private float currentSpeed;                // Current forward speed
    private float targetSpeed;                 // Target forward speed
    private float speedVelocity = 0.0f;        // Velocity reference for SmoothDamp

    private float currentWheelieAngle = 0.0f;  // Current wheelie angle
    private float targetWheelieAngle = 0.0f;   // Target wheelie angle
    private float wheelieAngleVelocity = 0.0f; // Velocity reference for SmoothDamp

    private float currentTiltAngle = 0.0f;     // Current tilt angle
    private float targetTiltAngle = 0.0f;      // Target tilt angle
    private float tiltAngleVelocity = 0.0f;    // Velocity reference for SmoothDamp

    private Quaternion initialModelRotation;   // Initial rotation of the modelPivot

    void Start()
    {
        // Initialize the player in the middle lane
        currentLane = roadSettings.numLanes / 2;
        targetXPosition = transform.position.x;

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

        // Handle lane change input
        if (!isChangingLane && cooldownTimer <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
        }

        // Handle lane changing movement
        if (isChangingLane)
        {
            laneChangeTimer += Time.deltaTime;
            float t = laneChangeTimer / laneChangeDuration;
            t = Mathf.Clamp01(t); // Ensure t doesn't go beyond 1

            // Smoothly interpolate between the start and target positions
            float newX = Mathf.Lerp(startXPosition, targetXPosition, t);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Reset lane change when complete
            if (t >= 1.0f)
            {
                isChangingLane = false;
                laneChangeTimer = 0.0f;
                cooldownTimer = laneChangeCooldown;
            }
        }

        // Handle wheelie input and speed adjustments
        HandleWheelieAndSpeed();

        // Handle tilt during lane changes
        HandleTilt();

        // Move the player forward
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);

        // Apply wheelie and tilt angles to modelPivot rotation
        if (modelPivot != null)
        {
            Quaternion wheelieRotation = Quaternion.Euler(0, 0, -currentWheelieAngle);
            Quaternion tiltRotation = Quaternion.Euler(currentTiltAngle, 0, 0);
            modelPivot.localRotation = initialModelRotation * wheelieRotation * tiltRotation;
        }
    }

    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            targetTiltAngle = -maxTiltAngle; // Tilt to the left
            StartLaneChange();
        }
    }

    void MoveRight()
    {
        if (currentLane < roadSettings.numLanes - 1)
        {
            currentLane++;
            targetTiltAngle = maxTiltAngle; // Tilt to the right
            StartLaneChange();
        }
    }

    void StartLaneChange()
    {
        startXPosition = transform.position.x;
        targetXPosition = GetLaneXPosition(currentLane);
        isChangingLane = true;
        laneChangeTimer = 0.0f;
    }

    float GetLaneXPosition(int laneIndex)
    {
        // Calculate X position based on lane index
        float centerLane = (roadSettings.numLanes - 1) / 2.0f;
        return (laneIndex - centerLane) * roadSettings.laneWidth;
    }

    void CheckWheelieAngle(float wheelieAngle)
    {
        bool doKillPlayer = wheelieAngle != Mathf.Clamp(wheelieAngle, 0, maxWheelieAngle);
        if (doKillPlayer) Debug.Log("Killed by failing the wheelie");
    }

    void HandleWheelieAndSpeed()
    {
        // Determine target speed and wheelie angle based on input
        if (Input.GetKey(KeyCode.W))
        {
            // Increase speed and wheelie angle
            targetSpeed = baseSpeed + maxSpeedBoost;
            targetWheelieAngle = maxWheelieAngle;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Decrease speed and wheelie angle
            targetSpeed = baseSpeed - maxSpeedBoost;
            targetWheelieAngle = -maxWheelieAngle;
        }
        else
        {
            // Return to base speed and level wheelie angle
            targetSpeed = baseSpeed;
            targetWheelieAngle = 0.0f;
        }

        // Smoothly adjust current speed towards target speed
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, speedAcceleration);

        // Clamp current speed
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed - maxSpeedBoost, baseSpeed + maxSpeedBoost);

        // Smoothly adjust wheelie angle towards target angle
        currentWheelieAngle = Mathf.SmoothDamp(currentWheelieAngle, targetWheelieAngle, ref wheelieAngleVelocity, wheelieSmoothTime);

        // Clamp wheelie angle
        if (demandWheelie)
        {
            CheckWheelieAngle(currentWheelieAngle);
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0, maxWheelieAngle);
        }
        else
        {
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, -maxWheelieAngle, maxWheelieAngle);
        }
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
}

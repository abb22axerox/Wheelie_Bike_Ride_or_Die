using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lane Settings")]
    public int numberOfLanes = 3;            // Total number of lanes
    public float laneWidth = 2.0f;           // Distance between each lane

    [Header("Movement Settings")]
    public float laneChangeCooldown = 0.5f;  // Cooldown time between lane changes
    public float laneChangeDuration = 0.5f;  // Time it takes to move to a new lane
    public float forwardSpeed = 10.0f;       // Speed at which the player moves forward

    private int currentLane;                 // Current lane index (0 is the leftmost lane)
    private float targetXPosition;           // Target X position after lane change
    private float startXPosition;            // Starting X position before lane change
    private bool isChangingLane = false;     // Is the player currently changing lanes
    private float laneChangeTimer = 0.0f;    // Timer for lane change interpolation
    private float cooldownTimer = 0.0f;      // Timer for lane change cooldown

    void Start()
    {
        // Initialize the player in the middle lane
        currentLane = numberOfLanes / 2;
        targetXPosition = transform.position.x;
    }

    void Update()
    {
        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Handle input if not currently changing lanes and cooldown has elapsed
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
        }

        // Move the player forward
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);

        // Reset lane change when complete
        if (isChangingLane && laneChangeTimer >= laneChangeDuration)
        {
            isChangingLane = false;
            laneChangeTimer = 0.0f;
            cooldownTimer = laneChangeCooldown;
        }
    }

    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            StartLaneChange();
        }
    }

    void MoveRight()
    {
        if (currentLane < numberOfLanes - 1)
        {
            currentLane++;
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
        float centerLane = (numberOfLanes - 1) / 2.0f;
        return (laneIndex - centerLane) * laneWidth;
    }
}

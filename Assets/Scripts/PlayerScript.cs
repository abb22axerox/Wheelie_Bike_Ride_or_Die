using System;
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Player settings
    public float laneChangeCooldown = 0.5f;
    public float laneChangeSpeed = 5f;
    public float forwardSpeed = 5f;

    public RoadManager roadManager;

    // Road position
    [NonSerialized] public int lane = 0; // Current lane of the player
    [NonSerialized] public int roadPosition = 0; // Player's position (increases over time)

    // Private variables
    private bool canChangeLane = true;

    void Start()
    {
        // Start moving forward and increasing position
        StartCoroutine(IncreasePositionOverTime());
    }

    void Update()
    {
        if (canChangeLane)
        {
            if (Input.GetKeyDown(KeyCode.A) && lane > 0)
            {
                StartCoroutine(ChangeLane(-1));
            }
            else if (Input.GetKeyDown(KeyCode.D) && lane < roadManager.numLanes)
            {
                StartCoroutine(ChangeLane(1));
            }
        }
    }

    private IEnumerator ChangeLane(int direction)
    {
        canChangeLane = false;
        lane += direction;

        Vector3 targetPosition = new Vector3(lane * roadManager.laneWidth, transform.position.y, transform.position.z);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneChangeSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        yield return new WaitForSeconds(laneChangeCooldown);
        canChangeLane = true;
    }

    private IEnumerator IncreasePositionOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds((float)(1.0f / Mathf.Max(forwardSpeed, 0.001f)));
            roadPosition++;
            Debug.Log(roadPosition);
        }
    }

    public void OnCrash()
    {
        Debug.Log("Player crashed. Handle game over logic here.");
    }
}

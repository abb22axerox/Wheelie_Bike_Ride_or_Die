using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) CollectSpeedUp();
    }

    void CollectSpeedUp()
    {
        Debug.Log("Rocket collected");
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.DoSpeedUp();
        Destroy(gameObject);
    }
}

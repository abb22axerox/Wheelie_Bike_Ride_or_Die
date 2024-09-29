using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDown : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Collect();
    }

    void Collect()
    {
        Debug.Log("SpeedUp collected");
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.DoRocket();
        Destroy(gameObject);
    }
}

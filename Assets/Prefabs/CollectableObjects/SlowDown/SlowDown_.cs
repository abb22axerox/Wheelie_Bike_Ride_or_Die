using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDown_ : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) CollectSlowDown();
    }

    void CollectSlowDown()
    {
        Debug.Log("SlowDown collected");
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.DoSlowDown();
        Destroy(gameObject);
    }
}

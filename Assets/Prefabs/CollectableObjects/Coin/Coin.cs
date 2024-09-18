using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Collect();
    }

    void Collect()
    {
        Debug.Log("Coin collected");
        Destroy(gameObject);
    }
}

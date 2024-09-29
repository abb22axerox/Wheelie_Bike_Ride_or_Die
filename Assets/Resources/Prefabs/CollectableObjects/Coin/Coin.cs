using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Coin : MonoBehaviour
{
    public ScoreManager scoreManager;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Collect();
    }
 
    void Collect()
    {
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        scoreManager.AddCoin(1);
        Debug.Log("Coin collected");
        Destroy(gameObject);
    }
}
 
 
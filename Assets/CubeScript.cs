using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public int myNum;  // Corrected the declaration of myNum
    private GenerateCube generate;
    private Renderer rend; 

    void Start() {
        generate = GetComponentInParent<GenerateCube>();
        rend = GetComponentInParent<Renderer>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player") {  // Ensure the tag matches the case used in Unity's tag manager
            generate.Message(myNum);
            rend.material.color = Color.green;
        }
    }
}

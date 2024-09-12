using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCube : MonoBehaviour
{
    public GameObject CubeObj;
    Vector3 pos;
    bool next;
    public float[] posX;
    public float[] posZ;
    int value = 1;
    public int lastpos = 1;

    void FixedUpdate()
    {
        StartCoroutine(WaitSys());
    }

    IEnumerator WaitSys()
    {
        yield return new WaitForSeconds(1f);
        next = true;
        Generate();
    }

    void Generate()
    {
        if (!next)
            return;

        int i = Random.Range(0, 3);  // Corrected "Random.range" to "Random.Range"
        pos.x = posX[i];
        pos.z += posZ[i];  // Corrected "pos.Z" to "pos.z"

        GameObject CubeClone = Instantiate(CubeObj, pos, CubeObj.transform.rotation);
        CubeClone.GetComponent<CubeScript>().myNum = value;
        CubeClone.transform.SetParent(this.transform);
        value += 1;
        next = false;
    }

    public void Message(int i)
    {
        if (lastpos == i)
        {
            lastpos += 1;
            Debug.Log("found!!");  // Corrected "Debug.log" to "Debug.Log"
        }
        else
        {
            Debug.Log("not found");  // Corrected "Debug.log" to "Debug.Log"
            Application.LoadLevel (0);
        }
    }
}


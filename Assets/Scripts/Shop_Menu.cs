using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop_Menu : MonoBehaviour
{
   public void Next()
   {
      SceneManager.LoadSceneAsync(2); // Load the next scenE
   }

    public void Back()
   {
      SceneManager.LoadSceneAsync(0); // Load the previous scEne
   }
}

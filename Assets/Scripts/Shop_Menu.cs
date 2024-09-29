using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop_Menu : MonoBehaviour
{
   public void Next()
   {
      SceneManager.LoadSceneAsync(2);
   }

    public void Back()
   {
      SceneManager.LoadSceneAsync(0);
   }

}


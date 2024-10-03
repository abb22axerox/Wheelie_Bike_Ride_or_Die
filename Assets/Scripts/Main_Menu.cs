using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
   public void PlayGame() // start game
   {
      SceneManager.LoadSceneAsync(1); // load scene 1
   }

   public void QuitGame() // close app
   {
      Application.Quit(); // quit the game
   }
}

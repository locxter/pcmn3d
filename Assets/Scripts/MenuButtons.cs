using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
  private void Awake()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  public void Play()
  {
    SceneManager.LoadScene("Game");
  }

  public void HighScore()
  {
    SceneManager.LoadScene("HighScoreView");
  }

  public void Quit()
  {
    Application.Quit();
  }
}


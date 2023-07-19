using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreViewButtons : MonoBehaviour
{
  private void Awake()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  public void Return()
  {
    SceneManager.LoadScene("Menu");
  }

  public void Quit()
  {
    Application.Quit();
  }
}


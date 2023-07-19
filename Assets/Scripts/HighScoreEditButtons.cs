using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreEditButtons : MonoBehaviour
{
  public TMP_InputField HighScoreNameInput;

  private void Awake()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  private void StoreName()
  {
    string name = HighScoreNameInput.text;
    if (string.IsNullOrWhiteSpace(name))
    {
      name = "nobody";
    }
    PlayerPrefs.SetString("HighScoreName", name);
  }

  public void Save()
  {
    StoreName();
    SceneManager.LoadScene("HighScoreView");
  }

  public void Quit()
  {
    StoreName();
    Application.Quit();
  }
}

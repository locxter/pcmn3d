using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalLives : MonoBehaviour
{
  public static int CurrentLives { get; private set; } = 3;
  public TextMeshProUGUI LiveLabel;
  
  private int _internalLives;

  private void Update()
  {
    if (_internalLives != CurrentLives)
    {
      _internalLives = CurrentLives;
      LiveLabel.text = "Lives: " + CurrentLives;
    }
  }

  public static void RemoveLive()
  {
    if (CurrentLives > 1)
    {
      CurrentLives -= 1;
      foreach (GameObject ghost in GameObject.FindGameObjectsWithTag("Ghost"))
      {
        //ghost.GetComponent<GhostMove>().Reset();
        ghost.GetComponent<GhostMoveAdvanced>().Reset();
        
      }
      GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().Reset();
      GlobalTime.ResetTime();
      GlobalMode.SetFrightenedMode(false);
    }
    else
    {
      int score = GlobalScore.CurrentScore;
      MenuMessage.Message = "GAME OVER\nScore: " + score + "\nTry again.";
      GlobalLevel.CurrentLevel = 0;
      GlobalScore.CurrentScore = 0;
      ResetLives();
      GlobalTime.ResetTime();
      GlobalMode.SetFrightenedMode(false);
      if (score > PlayerPrefs.GetInt("HighScorePoints", 0))
      {
        PlayerPrefs.SetInt("HighScorePoints", score);
        SceneManager.LoadScene("HighScoreEdit");
      }
      else
      {
        SceneManager.LoadScene("Menu");
      }
    }
  }

  public static void ResetLives()
  {
    CurrentLives = 3;
  }
}

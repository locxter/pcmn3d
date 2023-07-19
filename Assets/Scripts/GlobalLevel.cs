using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalLevel : MonoBehaviour
{
  public static int CurrentLevel = 1;
  public int MaxLevel = 256;
  public TextMeshProUGUI LevelLabel;
  private int _internalLevel;
  
	private void Update()
	{
		if (_internalLevel != CurrentLevel)
		{
      _internalLevel = CurrentLevel;
      LevelLabel.text = "Level: " + CurrentLevel;
    }
    if (GameObject.FindGameObjectsWithTag("Pellet").Length + GameObject.FindGameObjectsWithTag("PowerPellet").Length == 0)
    {
      if (CurrentLevel == MaxLevel)
      {
        int score = GlobalScore.CurrentScore;
        MenuMessage.Message = "GAME FINISHED\nScore: " + score + "\nCongratulations!";
        CurrentLevel = 0;
        GlobalScore.CurrentScore = 0;
        GlobalLives.ResetLives();
        GlobalTime.ResetTime();
        GlobalMode.SetFrightenedMode(false);
        if (score > PlayerPrefs.GetInt("HighScorePoints", 0))
        {
          PlayerPrefs.SetInt("HighScorePoints", score);
          SceneManager.LoadScene("HighScoreEdit");
        } else
        {
          SceneManager.LoadScene("Menu");
        }
      }
      else
      {
        MenuMessage.Message = "Level " + GlobalLevel.CurrentLevel + " finished.\nScore: " + GlobalScore.CurrentScore + "\nReady to continue.";
        CurrentLevel += 1;
        GlobalTime.ResetTime();
        GlobalMode.SetFrightenedMode(false);
        SceneManager.LoadScene("Menu");
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreViewMessage : MonoBehaviour
{
  public TextMeshProUGUI MessageLabel;

  private void Start()
  {
    int points = PlayerPrefs.GetInt("HighScorePoints", 0);
    string name = PlayerPrefs.GetString("HighScoreName", "nobody");
    MessageLabel.text = points + " points\nby\n" + name;
  }
}

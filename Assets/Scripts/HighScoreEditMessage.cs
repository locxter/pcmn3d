using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreEditMessage : MonoBehaviour
{
  public TextMeshProUGUI MessageLabel;

  private void Start()
  {
    int points = PlayerPrefs.GetInt("HighScorePoints", 0);
    MessageLabel.text = points + " points\nby";
  }
}

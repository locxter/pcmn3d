using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalScore : MonoBehaviour
{
  public static int CurrentScore;
  public TextMeshProUGUI ScoreLabel;
  private int _internalScore;

  private void Update()
  {
    if (_internalScore != CurrentScore)
    {
      _internalScore = CurrentScore;
      ScoreLabel.text = "Score:\n" + CurrentScore;
    }
  }
}

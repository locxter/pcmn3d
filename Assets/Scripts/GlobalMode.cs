using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMode : MonoBehaviour
{
  public static Mode CurrentMode { get; private set; }
  public TextMeshProUGUI ModeLabel;

  private Mode _internalMode;

  public enum Mode
  {
    Chase,
    Scatter,
    Frightened
  }

  private void Start()
  {
    CurrentMode = Mode.Scatter;
  }

  private void Update()
	{
    if (_internalMode != CurrentMode)
    {
      _internalMode = CurrentMode;
      ModeLabel.text = "Mode:\n";
      switch (CurrentMode)
      {
        case Mode.Chase:
          ModeLabel.text += "Chase";
          break;
        case Mode.Scatter:
          ModeLabel.text += "Scatter";
          break;
        case Mode.Frightened:
          ModeLabel.text += "Frightened";
          break;
      }
    }
    SelectCorrectMode();
  }

  private static void SelectCorrectMode()
  {
    if (CurrentMode != Mode.Frightened)
    {
      Mode requiredMode;
      if (GlobalLevel.CurrentLevel < 2)
      {
        if (GlobalTime.CurrentTime <= 7)
        {
          requiredMode = Mode.Scatter;
        } else if (GlobalTime.CurrentTime <= 27)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 34)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 54)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 59)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 79)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 84)
        {
          requiredMode = Mode.Scatter;
        }
        else
        {
          requiredMode = Mode.Chase;
        }
      } else if (GlobalLevel.CurrentLevel >= 2 && GlobalLevel.CurrentLevel < 5)
      {
        if (GlobalTime.CurrentTime <= 7)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 27)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 34)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 54)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 59)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 1092)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 1092 + (1 / 60))
        {
          requiredMode = Mode.Scatter;
        }
        else
        {
          requiredMode = Mode.Chase;
        }
      } else
      {
        if (GlobalTime.CurrentTime <= 5)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 25)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 30)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 50)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 55)
        {
          requiredMode = Mode.Scatter;
        }
        else if (GlobalTime.CurrentTime <= 1092)
        {
          requiredMode = Mode.Chase;
        }
        else if (GlobalTime.CurrentTime <= 1092 + (1 / 60))
        {
          requiredMode = Mode.Scatter;
        }
        else
        {
          requiredMode = Mode.Chase;
        }
      }
      if (requiredMode != CurrentMode)
      {
        CurrentMode = requiredMode;
      }
    }
  }

  public static void SetFrightenedMode(bool enable)
  {
    if (CurrentMode != Mode.Frightened && enable)
    {
      GlobalTime.PauseTime();
      CurrentMode = Mode.Frightened;
    } else if (CurrentMode == Mode.Frightened && !enable)
    {
      GlobalTime.ResumeTime();
      GhostCollide.ResetGhostsEaten();
      CurrentMode = Mode.Scatter;
      SelectCorrectMode();
    }
  }
}

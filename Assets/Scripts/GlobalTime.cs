using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{
  public static float CurrentTime { get; private set; }

  private static bool _isActive;

  private void Start()
  {
    CurrentTime = 0;
    _isActive = true;
  }

  private void FixedUpdate()
  {
    if (_isActive)
    {
      CurrentTime += Time.fixedDeltaTime;
    }
  }

  public static void PauseTime()
  {
    _isActive = false;
  }

  public static void ResumeTime()
  {
    _isActive = true;
  }

  public static void ResetTime()
  {
    CurrentTime = 0;
  }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostCollide : MonoBehaviour
{
  public static int GhostsEaten { get; private set; } = 0;
  public AudioSource GhostKillAudio;
  public int ScoreIncrement = 200;

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Player")
    {
      if (GlobalMode.CurrentMode == GlobalMode.Mode.Frightened)
      {
        if (GhostKillAudio != null & GhostKillAudio.enabled)
        {
          GhostKillAudio.Play();
        }
        GlobalScore.CurrentScore += ScoreIncrement * (int)System.Math.Pow(2, GhostsEaten + 1);
        GhostsEaten++;
        //gameObject.GetComponent<GhostMove>().Reset();
        gameObject.GetComponent<GhostMoveAdvanced>().Reset();
        if (GhostsEaten == 4)
        {
          GlobalMode.SetFrightenedMode(false);
        }
        } else
      {
        GlobalLives.RemoveLive();
      }
    }
  }

  public static void ResetGhostsEaten() {
    GhostsEaten = 0;
  }
}

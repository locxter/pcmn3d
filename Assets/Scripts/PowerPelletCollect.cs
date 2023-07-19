using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerPelletCollect : MonoBehaviour
{
  public GameObject MeshTarget;
  public AudioSource PowerPelletAudio;
  public int ScoreIncrement = 50;
  public float FrightenedPeriod = 20;
  public float FrightenedPeriodDecrement = 1;
  public int MaxFrightenedLevel = 18;

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Player")
    {
      if (PowerPelletAudio != null & PowerPelletAudio.enabled)
      {
        PowerPelletAudio.Play();
      }
      GlobalScore.CurrentScore += ScoreIncrement;
      if (GlobalLevel.CurrentLevel <= MaxFrightenedLevel)
      {
        GlobalMode.SetFrightenedMode(true);
      }
      gameObject.GetComponent<Collider>().enabled = false;
      MeshTarget.GetComponent<MeshRenderer>().enabled = false;
      StartCoroutine(DestroyWithDelay(GlobalLevel.CurrentLevel));
    }
  }
  private IEnumerator DestroyWithDelay(int level)
  {
    if (level <= MaxFrightenedLevel)
    {
      yield return new WaitForSeconds(FrightenedPeriod - (FrightenedPeriodDecrement * level));
      GlobalMode.SetFrightenedMode(false);
    }
    Destroy(gameObject);
  }
}

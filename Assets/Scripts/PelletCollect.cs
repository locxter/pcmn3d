using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PelletCollect : MonoBehaviour
{
  public AudioSource PelletAudio;
  public int ScoreIncrement = 10;

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Player")
    {
      if (PelletAudio != null & PelletAudio.enabled)
      {
        PelletAudio.Play();
      }
      GlobalScore.CurrentScore += ScoreIncrement;
      Destroy(gameObject);
    }
  }
}

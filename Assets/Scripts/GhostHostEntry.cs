using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostHostEntry : MonoBehaviour
{
  public float PlayerCollisionZ;
  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Player")
    {
      Vector3 destination = col.gameObject.transform.position;
      destination.z = PlayerCollisionZ;
      col.gameObject.GetComponent<PlayerControl>().Teleport(destination);
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
  public Vector3 Destination;

  private void OnTriggerEnter(Collider col)
  {
    if (Destination != Vector3.zero)
    {
      switch (col.gameObject.tag)
      {
        case "Player":
          col.gameObject.GetComponent<PlayerControl>().Teleport(Destination);
          break;
        case "Ghost":
          col.gameObject.GetComponent<GhostMoveAdvanced>().Teleport(Destination);
          break;
      }
    }
  }
}

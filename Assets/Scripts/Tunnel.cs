using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Ghost")
    {
      col.gameObject.GetComponent<GhostMoveAdvanced>().SetInTunnel(true);
    }
  }
  private void OnTriggerExit(Collider col)
  {
    if (col.gameObject.tag == "Ghost")
    {
      col.gameObject.GetComponent<GhostMoveAdvanced>().SetInTunnel(false);
    }
  }
}

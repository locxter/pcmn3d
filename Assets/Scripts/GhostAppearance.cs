using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAppearance : MonoBehaviour
{
  public GameObject MeshTarget;
  public Material DefaultMaterial;
  public Material FrightenedMaterial;

  private bool _isFrightened = false;
  private Renderer _renderer;

  private void Awake()
  {
    _renderer = MeshTarget.GetComponent<Renderer>();
  }

  private void Update()
  {
    if (GlobalMode.CurrentMode == GlobalMode.Mode.Frightened && !_isFrightened)
    {
      _renderer.material = FrightenedMaterial;
      _isFrightened = true;
    } else if (GlobalMode.CurrentMode != GlobalMode.Mode.Frightened && _isFrightened)
    {
      _renderer.material = DefaultMaterial;
      _isFrightened = false;
    }
  }
}

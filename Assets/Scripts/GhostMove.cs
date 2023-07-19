using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class GhostMove : MonoBehaviour
{
	public Vector3[] ActiveCorners;
  public Vector3[] InactiveCorners;
	public float InitialSpeed = 3;
  public float SpeedIncrement = 1;
  public int MaxSpeedChangeLevel = 21;
  public float ActivationDelay = 10;

  private int _nextActiveCornerIndex = 1;
  private int _nextInactiveCornerIndex = 1;
  private bool _isActive = false;

  private void Start()
  {
    StartCoroutine(ActivateWithDelay());
  }

  // Remember these two things:
  // 1. Add the starting position as one of the corners
  // 2. Each corner should only differ in a single coordinate (x or z, y will be ignored)
  private void FixedUpdate()
  {
    float speed = InitialSpeed + (SpeedIncrement * System.Math.Min(GlobalLevel.CurrentLevel, MaxSpeedChangeLevel));
    if (GlobalMode.CurrentMode == GlobalMode.Mode.Frightened)
    {
      speed /= 2;
    }
    Vector3 nextCorner = Vector3.zero;
    if (InactiveCorners.Length > 1 && !_isActive)
    {
      nextCorner = InactiveCorners[_nextInactiveCornerIndex];
      MoveToCorner(speed, nextCorner);
      if (IsCornerReached(nextCorner))
      {
        _nextInactiveCornerIndex = (_nextInactiveCornerIndex + 1) % InactiveCorners.Length;
      }
    }
		if (ActiveCorners.Length > 1 && _isActive)
		{
      nextCorner = ActiveCorners[_nextActiveCornerIndex];
      MoveToCorner(speed, nextCorner);
      if (IsCornerReached(nextCorner))
      {
        transform.position = ActiveCorners[_nextActiveCornerIndex];
        _nextActiveCornerIndex = (_nextActiveCornerIndex + 1) % ActiveCorners.Length;
      }
    }
  }

  private void MoveToCorner(float speed, Vector3 corner)
  {
    if (transform.position.x != corner.x)
    {
      if (corner.x > transform.position.x)
      {
        transform.Translate(Vector3.right * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
      }
      else
      {
        transform.Translate(Vector3.left * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
      }
    }
    if (transform.position.z != corner.z)
    {
      if (corner.z > transform.position.z)
      {
        transform.Translate(Vector3.forward * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
      }
      else
      {
        transform.Translate(Vector3.back * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
      }
    }
  }

  private bool IsCornerReached(Vector3 corner)
  {
    if (transform.position.x > corner.x - 0.1 && transform.position.x < corner.x + 0.1 && transform.position.z > corner.z - 0.1 && transform.position.z < corner.z + 0.1)
    {
      transform.position = corner;
      return true;
    } else
    {
      return false;
    }
  }

  private void Deactivate()
  {
    _isActive = false;
    if (InactiveCorners.Length > 1)
    {
      _nextInactiveCornerIndex = 1;
      transform.position = InactiveCorners[0];
    } else if (ActiveCorners.Length > 1)
    {
      transform.position = ActiveCorners[0];
    }
  }

  private IEnumerator ActivateWithDelay()
  {
    yield return new WaitForSeconds(ActivationDelay);
    _isActive = true;
    if (ActiveCorners.Length > 1)
    {
      _nextActiveCornerIndex = 1;
      transform.position = ActiveCorners[0];
    } else if (InactiveCorners.Length > 1)
    {
      transform.position = InactiveCorners[0];
    }
  }

  public void Reset()
  {
    Deactivate();
    StartCoroutine(ActivateWithDelay());
  }
}

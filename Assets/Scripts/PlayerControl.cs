using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
  public CharacterController CharacterController;
  public float InitialSpeed = 3;
  public float SpeedIncrement = 1;
  public int MaxSpeedChangeLevel = 21;
  public Transform CameraTransform;
  public float MouseSensitivity = 2f;
  public float UpLimit = -50;
  public float DownLimit = 50;
  public Vector3 RotatedDirection {  get; private set; }

  private float _horizontalRotation;
  private Vector3 _direction;
  private Vector3 _destination = Vector3.zero;
  private Vector3 _startPosition = Vector3.zero;

  private void Awake()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    _startPosition = transform.position;
  }

  private void FixedUpdate()
  {
    Move();
    Rotate();
    RotatedDirection = Quaternion.Euler(0, _horizontalRotation, 0) * _direction;
  }

  private void Rotate()
  {
    _horizontalRotation = Input.GetAxis("Mouse X");
    float verticalRotation = Input.GetAxis("Mouse Y");
    transform.Rotate(0, _horizontalRotation * MouseSensitivity, 0);
    CameraTransform.Rotate(-verticalRotation * MouseSensitivity, 0, 0);
    Vector3 currentRotation = CameraTransform.localEulerAngles;
    if (currentRotation.x > 180)
    {
      currentRotation.x -= 360;
    }
    currentRotation.x = Mathf.Clamp(currentRotation.x, UpLimit, DownLimit);
    CameraTransform.localRotation = Quaternion.Euler(currentRotation);
  }

  private void Move()
  {
    float horizontalMove = Input.GetAxis("Horizontal");
    float verticalMove = Input.GetAxis("Vertical");
    _direction = (transform.forward * verticalMove) + (transform.right * horizontalMove);
    if (_destination != Vector3.zero)
    {
      transform.position = _destination;
      _destination = Vector3.zero;
      return;
    }
    CharacterController.Move(_direction * (InitialSpeed + (SpeedIncrement * Math.Min(GlobalLevel.CurrentLevel, MaxSpeedChangeLevel))) * Time.fixedDeltaTime);
  }

  public void Teleport(Vector3 destination)
  {
    _destination = destination;
  }

  public void Reset()
  {
    _destination = _startPosition;
  }
}

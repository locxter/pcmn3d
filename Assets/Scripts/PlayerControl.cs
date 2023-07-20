using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

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
  private bool _forwardPressed = false;
  private bool _leftPressed = false;
  private bool _backPressed = false;
  private bool _rightPressed = false;

  private void Awake()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    _startPosition = transform.position;
    EnhancedTouchSupport.Enable();
  }

  private void FixedUpdate()
  {
    Move();
    Rotate();
    RotatedDirection = Quaternion.Euler(0, _horizontalRotation, 0) * _direction;
  }

  private void Rotate()
  {
    Mouse mouse = Mouse.current;
    _horizontalRotation = mouse.delta.x.ReadValue();
    float verticalRotation = mouse.delta.y.ReadValue();
    #if (UNITY_ANDROID || UNITY_IOS)
        float speedCorrection = 1.25f;
    #else
        float speedCorrection = 0.05f;
    #endif
    transform.Rotate(0, _horizontalRotation * speedCorrection * MouseSensitivity, 0);
    CameraTransform.Rotate(-verticalRotation * speedCorrection * MouseSensitivity, 0, 0);
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
    Keyboard keyboard = Keyboard.current;
    // Forward
    if (keyboard.wKey.wasPressedThisFrame && !_forwardPressed)
    {
      _forwardPressed = true;
    } else if (keyboard.wKey.wasReleasedThisFrame && _forwardPressed)
    {
      _forwardPressed = false;
    }
    // Left
    if (keyboard.aKey.wasPressedThisFrame && !_leftPressed)
    {
      _leftPressed = true;
    }
    else if (keyboard.aKey.wasReleasedThisFrame && _leftPressed)
    {
      _leftPressed = false;
    }
    // Back
    if (keyboard.sKey.wasPressedThisFrame && !_backPressed)
    {
      _backPressed = true;
    }
    else if (keyboard.sKey.wasReleasedThisFrame && _backPressed)
    {
      _backPressed = false;
    }
    // Right
    if (keyboard.dKey.wasPressedThisFrame && !_rightPressed)
    {
      _rightPressed = true;
    }
    else if (keyboard.dKey.wasReleasedThisFrame && _rightPressed)
    {
      _rightPressed = false;
    }
    float verticalMove = (_forwardPressed ? 1 : 0) + (_backPressed ? -1 : 0);
    float horizontalMove = (_rightPressed ? 1 : 0) + (_leftPressed ? -1 : 0);
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

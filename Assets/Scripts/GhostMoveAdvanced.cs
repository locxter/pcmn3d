using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class GhostMoveAdvanced : MonoBehaviour
{
  public Vector3[] ValidPositions;
  public Vector3[] Corners;
  public Vector3[] NoForwardCorners = new Vector3[4];
  public Vector3[] ActivationCorners;
  public Vector3 ScatterTarget;
  [SerializeField]
  public Personality GhostPersonality;
  public float InitialSpeed = 3;
  public float SpeedIncrement = 1;
  public int MaxSpeedChangeLevel = 21;
  public float FrightenedSlowDown = .5f;
  public float TunnelSlowDown = .5f;
  public bool InTunnel { get; private set; } = false;

  private Dictionary<Vector3, List<Vector3>> _cornerDirections = new Dictionary<Vector3, List<Vector3>>();
  private Vector3 _target;
  private Vector3 _direction;
  private bool _alreadyOnCorner;
  private GlobalMode.Mode _oldMode;
  private Vector3 _destination = Vector3.zero;
  private Vector3 _startPosition = Vector3.zero;
  private Status _status;
  private int _nextActivationCornerIndex = 0;

  public enum Personality
  {
    Blinky,
    Pinky,
    Inky,
    Clyde
  }

  private enum Status
  {
    Inactive,
    Activating,
    Active
  }

  // All corners and object are required to have the same y values of 3, otherwise things are likely to fall apart
  private void Awake()
  {
    // Find possible movement directions for each corner
    foreach (Vector3 corner in Corners)
    {
      List<Vector3> directions = new List<Vector3>();
      // Right
      Collider[] colliders = Physics.OverlapBox(corner + new Vector3(2.5f, 0, 0), Vector3.zero);
      if (!CollidesWithTag(colliders, "Wall"))
      {
        directions.Add(Vector3.right);
      }
      // Back
      colliders = Physics.OverlapBox(corner + new Vector3(0, 0, -2.5f), Vector3.zero);
      if (!CollidesWithTag(colliders, "Wall"))
      {
        directions.Add(Vector3.back);
      }
      // Left
      colliders = Physics.OverlapBox(corner + new Vector3(-2.5f, 0, 0), Vector3.zero);
      if (!CollidesWithTag(colliders, "Wall"))
      {
        directions.Add(Vector3.left);
      }
      // Forward (unless in four special positions where the ghosts can't move in this direction)
       colliders = Physics.OverlapBox(corner + new Vector3(0, 0, 2.5f), Vector3.zero);
      if (!CollidesWithTag(colliders, "Wall") && corner != NoForwardCorners[0] && corner != NoForwardCorners[1] && corner != NoForwardCorners[2] && corner != NoForwardCorners[3])
      {
        directions.Add(Vector3.forward);
      }
      _cornerDirections.Add(corner, directions);
    }
    _startPosition = transform.position;
  }

  private void Start()
  {
    _direction = Vector3.left;
    if (ActivationCorners.Length > 0)
    {
      _target = ActivationCorners.Last() + (9 * _direction);
    } else
    {
      _target = _startPosition + (9 * _direction);
    }
    _alreadyOnCorner = false;
    _oldMode = GlobalMode.Mode.Scatter;
    _status = Status.Inactive;
    _nextActivationCornerIndex = 0;
    transform.localRotation = Quaternion.Euler(0, 0, 0);
  }

  private void FixedUpdate()
  {
    // Teleport if needed
    if (_destination != Vector3.zero)
    {
      transform.position = _destination;
      _destination = Vector3.zero;
      return;
    }
    // Calculate speed
    float speed = InitialSpeed + (SpeedIncrement * System.Math.Min(GlobalLevel.CurrentLevel, MaxSpeedChangeLevel));
    if (GlobalMode.CurrentMode == GlobalMode.Mode.Frightened)
    {
      speed *= FrightenedSlowDown;
    }
    if (InTunnel)
    {
      speed *= TunnelSlowDown;
    }
    // Move and do other stuff depending on current status
    switch (_status)
    {
      case Status.Inactive:
        if (GhostPersonality == Personality.Blinky || GhostPersonality == Personality.Pinky)
        {
          // Activate immediately
          _status = Status.Activating;
        } else if (GhostPersonality == Personality.Inky && GameObject.FindGameObjectsWithTag("Pellet").Length + GameObject.FindGameObjectsWithTag("PowerPellet").Length <= /*214*/ 240)
        {
          // Activate after 30 pellets have been eaten
          _status = Status.Activating;
        } else if (GhostPersonality == Personality.Clyde && GameObject.FindGameObjectsWithTag("Pellet").Length + GameObject.FindGameObjectsWithTag("PowerPellet").Length <= 164)
        {
          // Activate after 80 pellets have been eaten
          _status = Status.Activating;
        }
        break;
      case Status.Activating:
        // Move through activation corners and then activate
        if (ActivationCorners.Length > 0)
        {
          Vector3 nextCorner = ActivationCorners[_nextActivationCornerIndex];
          MoveToCorner(speed, nextCorner);
          if (IsPositionReached(nextCorner))
          {
            transform.position = nextCorner;
            _nextActivationCornerIndex++;
            if (_nextActivationCornerIndex == ActivationCorners.Length)
            {
              _status = Status.Active;
              _oldMode = GlobalMode.CurrentMode;
            }
          }
        } else
        {
          _status = Status.Active;
          _oldMode = GlobalMode.CurrentMode;
        }
        break;
      case Status.Active:
        // Rotate face to current direction
        RotateFaceToDirection(_direction);
        // Move in current direction
        transform.Translate(_direction * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
        // If target reached, determine new target
        if (IsPositionReached(_target) && GlobalMode.CurrentMode != GlobalMode.Mode.Frightened)
        {
          DetermineNewTarget();
        }
        // If on corner, determine new direction
        bool onCorner = false;
        Vector3 currentCorner = Vector3.zero;
        foreach (var corner in Corners)
        {
          if (IsPositionReached(corner))
          {
            onCorner = true;
            currentCorner = corner;
            break;
          }
        }
        if (onCorner && !_alreadyOnCorner)
        {
          GameObject pacman = GameObject.FindGameObjectWithTag("Player");
          Vector3 pacmanPosition = GetValidPosition(pacman.transform.position);
          // Ensure Clyde shies away, when getting to close to Pacman
          if (GhostPersonality == Personality.Clyde && GlobalMode.CurrentMode == GlobalMode.Mode.Chase && EuclideanDistance(GetValidPosition(transform.position), pacmanPosition) < 16)
          {
            DetermineNewDirection(currentCorner, ScatterTarget);
          } else
          {
            DetermineNewDirection(currentCorner, _target);
          }
          _alreadyOnCorner = true;
        }
        else if (!onCorner && _alreadyOnCorner)
        {
          _alreadyOnCorner = false;
        }
        // If mode changed from chase or scatter to any other mode, reverse direction and determine new target
        if (GlobalMode.CurrentMode != _oldMode && (_oldMode == GlobalMode.Mode.Scatter || _oldMode == GlobalMode.Mode.Chase))
        {
          _direction *= -1;
          DetermineNewTarget();
          _oldMode = GlobalMode.CurrentMode;
        }
        break;
    }
  }

  private float EuclideanDistance(Vector3 position1, Vector3 position2)
  {
    return (float) System.Math.Sqrt(System.Math.Pow(position2.x - position1.x, 2) + System.Math.Pow(position2.z - position1.z, 2));
  }

  private bool IsPositionReached(Vector3 position)
  {
    float error = 0.1f + (float) (SpeedIncrement * 0.05 * System.Math.Min(GlobalLevel.CurrentLevel, MaxSpeedChangeLevel));
    if (transform.position.x > position.x - error && transform.position.x < position.x + error && transform.position.z > position.z - error && transform.position.z < position.z + error)
    {
      if (!_alreadyOnCorner)
      {
        transform.position = position;
      }
      return true;
    }
    else
    {
      return false;
    }
  }

  private bool CollidesWithTag(Collider[] colliders, string tag)
  {
    bool isColliding = false;
    foreach (Collider col in colliders)
    {
      if (col.gameObject.tag == tag)
      {
        isColliding = true;
        break;
      }
    }
    return isColliding;
  }

  private void MoveToCorner(float speed, Vector3 corner)
  {
    if (transform.position.x != corner.x)
    {
      if (corner.x > transform.position.x)
      {
        transform.Translate(Vector3.right * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
        RotateFaceToDirection(Vector3.right);
      }
      else
      {
        transform.Translate(Vector3.left * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
        RotateFaceToDirection(Vector3.left);
      }
    }
    if (transform.position.z != corner.z)
    {
      if (corner.z > transform.position.z)
      {
        transform.Translate(Vector3.forward * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
        RotateFaceToDirection(Vector3.forward);
      }
      else
      {
        transform.Translate(Vector3.back * speed * UnityEngine.Time.fixedDeltaTime, Space.World);
        RotateFaceToDirection(Vector3.back);
      }
    }
  }

  private Vector3 GetValidPosition(Vector3 position)
  {
    /*
    position.x = (float) System.Math.Floor(position.x);
    position.z = (float) System.Math.Floor(position.z);
    if (position.x % 2 == 1)
    {
      position.x -= .5f;
    } else
    {
      position.x += .5f;
    }
    if (position.z % 2 == 1)
    {
      position.z -= .5f;
    }
    else
    {
      position.z += .5f;
    }
    return position;
    */
    // Choose the valid position, which is closest to the actual unaligned position
    float minDistance = Mathf.Infinity;
    Vector3 closestValidPosition = Vector3.zero;
    foreach (Vector3 validPosition in ValidPositions)
    {
      float distance = EuclideanDistance(position, validPosition);
      if (distance <= minDistance)
      {
        minDistance = distance;
        closestValidPosition = validPosition;
        if (minDistance < 1)
        {
          break;
        }
      }
      
    }
    return closestValidPosition;
  }

  private Vector3 GetValidDirection(Vector3 direction)
  {
    if (direction.z >= 0.5)
    {
      // Up
      return Vector3.forward;
    } else if (direction.x <= -0.5)
    {
      // Left
      return Vector3.left;
    } else if (direction.z <= -0.5)
    {
      // Down
      return Vector3.back;
    } else if (direction.x >= 0.5)
    {
      // Right
      return Vector3.right;
    } else
    {
      // Nothing
      return Vector3.zero;
    }
  }

  private void DetermineNewTarget()
  {
    if (GlobalMode.CurrentMode == GlobalMode.Mode.Chase)
    {
      // Select dynamic tiles depending on the ghost's personality and some other variables
      GameObject pacman = GameObject.FindGameObjectWithTag("Player");
      Vector3 pacmanPosition = GetValidPosition(pacman.transform.position);
      Vector3 pacmanDirection = GetValidDirection(pacman.GetComponent<PlayerControl>().RotatedDirection);
      switch (GhostPersonality)
      {
        case Personality.Blinky:
          // Set Pacman's current position as the new target
          _target = pacmanPosition;
          break;
        case Personality.Pinky:
          // Set Pacman's position 4 tiles into the future as the new target (also emulate the overflow error, when going up)
          if (pacmanDirection == Vector3.forward)
          {
            _target = GetValidPosition(pacmanPosition + (8 * pacmanDirection) + (8 * Vector3.left));
          } else
          {
            _target = GetValidPosition(pacmanPosition + (8 * pacmanDirection));
          }
          break;
        case Personality.Inky:
          // Set Blinky's position plus two times the difference vector from Blinky to Pacman's position 2 tiles into the future as the new target (also emulate the overflow error, when going up)
          GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
          Vector3 blinkyPosition = Vector3.zero;
          foreach(GameObject ghost in ghosts)
          {
            if (ghost.GetComponent<GhostMoveAdvanced>().GhostPersonality == Personality.Blinky)
            {
              blinkyPosition = GetValidPosition(ghost.transform.position);
              break;
            }
          }
          if (pacmanDirection == Vector3.forward)
          {
            _target = GetValidPosition(blinkyPosition + (((pacmanPosition + (4 * pacmanDirection) + (4 * Vector3.left)) - blinkyPosition) * 2));
          }
          else
          {
            _target = GetValidPosition(blinkyPosition + (((pacmanPosition + (4 * pacmanDirection)) - blinkyPosition) * 2));
          }
          break;
        case Personality.Clyde:
          // Like Blinky until 8 tiles away, then just select the scatter mode target (done in the FixedUpdate function)
          _target = pacmanPosition;
          break;
      }
    } else if (GlobalMode.CurrentMode == GlobalMode.Mode.Scatter)
    {
      // Select predefined tiles outside the play area to enter fixed loops
      _target = ScatterTarget;
    }
  }

  private void DetermineNewDirection(Vector3 corner, Vector3 target)
  {
    List<Vector3> possibleDirections = new List<Vector3>(_cornerDirections[corner]); // Might not copy it, would have to change then
    // Prevent the ghost from reversing
    for (int i = 0; i < possibleDirections.Count; i++)
    {
      if (possibleDirections[i] == _direction * -1)
      {
        possibleDirections.RemoveAt(i);
        break;
      }
    }
    if (GlobalMode.CurrentMode == GlobalMode.Mode.Frightened)
    {
      // Choose random direction
      _direction = possibleDirections[new System.Random().Next(possibleDirections.Count)];
    } else
    {
      // Choose the direction, which on the first step reduces the distance to the target the most
      float minDistance = Mathf.Infinity;
      Vector3 newDirection = Vector3.zero;
      foreach (Vector3 possibleDirection in possibleDirections)
      {
        float distance = EuclideanDistance(GetValidPosition(transform.position + possibleDirection), target);
        if (distance <= minDistance)
        {
          minDistance = distance;
          newDirection = possibleDirection;
        }
      }
      _direction = newDirection;
    }
  }

  private void RotateFaceToDirection(Vector3 direction)
  {
    // Rotate the ghost so his eyes also face in the movement direction
    if (direction == Vector3.forward)
    {
      transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    else if (direction == Vector3.left)
    {
      transform.localRotation = Quaternion.Euler(0, 270, 0);
    }
    else if (direction == Vector3.back)
    {
      transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    else if (direction == Vector3.right)
    {
      transform.localRotation = Quaternion.Euler(0, 90, 0);
    }
  }
    public void Teleport(Vector3 destination)
  {
    _destination = destination;
  }

  public void SetInTunnel(bool enable)
  {
    InTunnel = enable;
  }

  public void Reset()
  {
    Teleport(_startPosition);
    FixedUpdate();
    Start();
  }
}

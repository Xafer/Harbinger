using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPath : MonoBehaviour
{
    [System.Serializable]
    public struct PathPoint
    {
        public Vector3 Point;
        public float Euler;

        public PathPoint(Vector3 point, float yAngle)
        {
            Point = point;
            Euler = yAngle;
        }
        
    }

    public enum PathTrigger { Active, Passive }
    public enum PathEntityType { Player, PlayerRobot, Monster, Humanoid }

    private Dictionary<Entity, float> _entityProgress = new Dictionary<Entity, float>();

    [SerializeField] private List<PathPoint> _pathPoints = new List<PathPoint>();
    [SerializeField] private PathTrigger _pathType = PathTrigger.Passive;
    [SerializeField] private float _angleRange = 60;
    [SerializeField] private float _angleStrength = 10;
    [SerializeField] private PathEntityType _entityType = PathEntityType.Player;
    [SerializeField] private int _lastPoint = 1;
    [SerializeField] private bool _oneWay = false;

    public Vector3 GetPosition(Entity entity, Vector3 movement)
    {
        float entityProgress = _entityProgress[entity];

        Vector3 currentLine = (_pathPoints[Mathf.CeilToInt(entityProgress)].Point) - (_pathPoints[Mathf.FloorToInt(entityProgress)].Point);

        float movementAmount = 0;

        if (movement.magnitude > 0.01f && currentLine.magnitude > 0)
        {
            float dot = _oneWay?    movement.z:
                                    Vector3.Dot((entity.transform.rotation * Quaternion.Euler(entity.GetXRotation(), 0, 0) * movement).normalized, transform.rotation * currentLine.normalized);

            movementAmount = (dot / currentLine.magnitude * entity.GetSpeed()) * Time.deltaTime;
        }

        entityProgress = Mathf.Clamp(entityProgress + movementAmount,0, _pathPoints.Count - 1);

        _entityProgress[entity] = entityProgress;

        if (entityProgress > 0 && entityProgress < _pathPoints.Count)
        {
            float angleMid = GetRotation(entityProgress);

            float distanceFromFront = Mathf.DeltaAngle(entity.transform.eulerAngles.y, angleMid);

            float angle = 0;

            if (Mathf.Abs(distanceFromFront) > _angleRange)
                angle = Mathf.Sign(distanceFromFront) * (Mathf.Abs(distanceFromFront) - _angleRange);

            //Mathf.DeltaAngle

            entity.transform.eulerAngles += new Vector3(0, angle * Time.deltaTime * _angleStrength, 0);

            return transform.rotation * Vector3.Lerp(_pathPoints[Mathf.FloorToInt(entityProgress)].Point, _pathPoints[Mathf.CeilToInt(entityProgress)].Point, entityProgress % 1.0f) + transform.position;
        }
        else
        {
            ExitPath(entity);
            return entity.transform.position;
        }
    }

    private float GetRotation(float n)
    {
        return Mathf.LerpAngle(_pathPoints[Mathf.FloorToInt(n)].Euler, _pathPoints[Mathf.CeilToInt(n)].Euler, n % 1.0f) + transform.eulerAngles.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pathType == PathTrigger.Passive)
        {
            switch (_entityType)
            {
                default:
                case PathEntityType.Player:
                    if (other.GetComponent<Player>() != null)
                        EnterPath(other.GetComponent<Entity>());
                    break;
                case PathEntityType.PlayerRobot:
                    if (other.GetComponent<PlayerRobot>() != null)
                        EnterPath(other.GetComponent<Entity>());
                    break;
                case PathEntityType.Monster:
                    if (other.GetComponent<Entity>() != null)
                        EnterPath(other.GetComponent<Entity>());
                    break;
            }
        }
        else if(_pathType == PathTrigger.Active)
        {
            EnterActive(other.GetComponent<Entity>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ExitPath(other.GetComponent<Entity>());
    }

    public void EnterActive(Entity pathedEntity)
    {
        pathedEntity.CurrentToggleablePath = this;
    }

    public void EnterPath(Entity pathedEntity)
    {
        if (pathedEntity != null)
        {
            pathedEntity.CurrentPath = this;
            Vector3 peLocalPosition = pathedEntity.transform.position - transform.position;

            int entityProgress = (peLocalPosition - transform.rotation* _pathPoints[0].Point).magnitude < (peLocalPosition - transform.rotation * _pathPoints[_pathPoints.Count-1].Point).magnitude ?0 : _pathType == PathTrigger.Passive? _pathPoints.Count - 2 : _lastPoint;

            PathPoint a = _pathPoints[entityProgress];
            PathPoint b = _pathPoints[entityProgress + 1];

            float distToA = (a.Point - peLocalPosition).magnitude;
            float distToB = (b.Point - peLocalPosition).magnitude;

            _entityProgress.Add(pathedEntity, Mathf.Clamp(entityProgress + (distToA / (distToA + distToB)),0,_pathPoints.Count));
        }
    }

    public void ExitPath(Entity pathedEntity)
    {
        if (pathedEntity != null)
        {
            pathedEntity.CurrentPath = null;
            pathedEntity.CurrentToggleablePath = null;
        }

        if (_entityProgress.ContainsKey(pathedEntity))
            _entityProgress.Remove(pathedEntity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (_pathPoints.Count > 1)
        {
            for (int i = 0; i < _pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(transform.rotation*_pathPoints[i].Point + transform.position, transform.rotation*_pathPoints[i+1].Point + transform.position);
                if (i > 0)
                    Gizmos.DrawCube(transform.rotation*_pathPoints[i].Point + transform.position, Vector3.one * 0.05f);
            }
            Gizmos.DrawSphere(transform.rotation*_pathPoints[_pathPoints.Count - 1].Point + transform.position, 0.1f);
        }

        if (_pathPoints.Count > 0)
            Gizmos.DrawSphere(transform.rotation*_pathPoints[0].Point + transform.position, 0.1f);

        foreach(PathPoint pp in _pathPoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + transform.rotation*pp.Point, transform.position + transform.rotation*pp.Point + transform.rotation * Quaternion.Euler(0, pp.Euler, 0) * Vector3.forward * 2);
        }

    }

    public bool IsPassive()
    {
        return _pathType == PathTrigger.Passive;
    }
}

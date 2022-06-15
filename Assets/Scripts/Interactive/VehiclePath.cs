using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehiclePath : MonoBehaviour
{
    [System.Serializable]
    public struct PathPoint
    {
        public Vector3 Position;
        public Vector3 Euler;
        public float SpeedFactor;
        public float ShakingFactor;
    }

    [System.Serializable]
    public class EventPoint
    {
        public float TriggerPoint;
        public bool EventTriggered;

        public UnityEvent TriggeredEvent;

        public EventPoint(float triggerPoint)
        {
            TriggerPoint = triggerPoint;
            EventTriggered = false;

            if (TriggeredEvent == null)
                TriggeredEvent = new UnityEvent();
        }
    }

    [SerializeField] private List<EventPoint> _events = new List<EventPoint>();

    [SerializeField] private List<PathPoint> _points = new List<PathPoint>();
    [SerializeField] private bool _loop = false;
    [SerializeField] private float _progress = 0;
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _shakingFactor = 1;

    [SerializeField] private float _fadeInAt = 0.2f;
    [SerializeField] private float _fadeOutAt = 15f;

    [SerializeField] private float _dialogAt = 2;

    [SerializeField] private float _followFactor = 0.2f;
    
    [SerializeField] private UnityEvent _fadeCameraEvent;
    [SerializeField] private UnityEvent _endPathEvent;

    void Awake()
    {
        if (_fadeCameraEvent == null)
            _fadeCameraEvent = new UnityEvent();

        if (_endPathEvent == null)
            _endPathEvent = new UnityEvent();
    }

    private void Update()
    {
        PathPoint a = _points[Mathf.FloorToInt(_progress)];
        PathPoint b = _points[Mathf.FloorToInt(_progress+1)];

        float dist = (a.Position - b.Position).magnitude;

        float speedFactor = (1 - _progress % 1) * a.SpeedFactor + (_progress % 1) * b.SpeedFactor;

        float distanceFactor = dist <= 0.01f? 0.01f : (_speed / dist) * Time.deltaTime * speedFactor;

        int progressFloored = Mathf.FloorToInt(_progress);

        _progress += distanceFactor;

        if (_progress >= _points.Count - 1)
        {
            _progress %= _points.Count - 1;
            _endPathEvent.Invoke();
        }

        float progress = (_progress+1) % (progressFloored+1);

        List<EventPoint> triggeredEvents = new List<EventPoint>();

        foreach(EventPoint ep in _events)
        {
            if(!ep.EventTriggered && ep.TriggerPoint < _progress)
            {
                triggeredEvents.Add(ep);
                ep.TriggeredEvent.Invoke();
            }
        }

        foreach (EventPoint ep in triggeredEvents)
            _events.Remove(ep);
         
        Vector3 targetPosition = Vector3.Lerp(a.Position, b.Position, progress) + Vector3.up * Random.Range(-a.ShakingFactor / 2, a.ShakingFactor / 2);

        transform.position = Vector3.Lerp(transform.position, targetPosition, _followFactor);

        Quaternion targetRotation = Quaternion.Slerp(Quaternion.Euler(a.Euler), Quaternion.Euler(b.Euler), progress) * Quaternion.Euler(Random.Range(-a.ShakingFactor, a.ShakingFactor), 0, Random.Range(-a.ShakingFactor, a.ShakingFactor));

        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, _followFactor);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_points.Count > 0)
            for (int i = 0; i < _points.Count; i++)
            {
                PathPoint pp = _points[i];

                Gizmos.DrawCube(pp.Position, Vector3.one * 0.2f);
                Vector3 rotPoint = pp.Position + Quaternion.Euler(pp.Euler) * Vector3.forward;
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(pp.Position, rotPoint);

                Gizmos.color = Color.yellow;

                if (i < _points.Count - 1)
                    Gizmos.DrawLine(pp.Position, _points[i+1].Position);

            }
    }
}

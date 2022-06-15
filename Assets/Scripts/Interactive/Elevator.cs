using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private List<float> _elevatorStops = new List<float>();
    [SerializeField] private float _boneHeight = 4;
    [SerializeField] private Transform _bone;
    [SerializeField] private float _elevatorSpeed = 1;
    [SerializeField] private bool _toggleMovement = false;

    private int _currentTarget;
    private float _currentHeight;

    public int MinStop = 2;
    // Start is called before the first frame update
    void Start()
    {
        _currentTarget = 0;
        _currentHeight = +_elevatorStops[0];
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.U))
            Move(true);
        if (Input.GetKey(KeyCode.J))
            Move(false);
    }

    void FixedUpdate()
    {
        float d = _elevatorStops[_currentTarget] - _currentHeight;
        if (_toggleMovement && Mathf.Abs(d) > 0.05f)
        {
            Move(d > 0);
        }
    }

    public void ChangeTarget(bool isUp)
    {
        float d = _elevatorStops[_currentTarget] - _currentHeight;
        if (Mathf.Abs(d) < 0.05f)
        {
            _currentTarget = Mathf.Clamp(_currentTarget + (isUp ? 1 : -1), 0, _elevatorStops.Count - 1);
        }
    }

    public void Move(bool isUp)
    {
        _currentHeight += (_elevatorSpeed * Time.deltaTime) * (isUp?1:-1);
        _currentHeight = Mathf.Clamp(_currentHeight, _elevatorStops[MinStop], _elevatorStops[0]);

        transform.position = new Vector3(transform.position.x, _currentHeight, transform.position.z);
        _bone.position = new Vector3(_bone.position.x, _boneHeight, _bone.position.z);
    }
}

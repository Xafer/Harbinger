using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : Entity
{
    [SerializeField] private Transform _cameraBone;
    private float _currentCameraRotation;
    private float _currentRobotRotation;

    [SerializeField] private Transform _robotMesh;

    private Vector3 _currentForward;
    private float _zRotation;

    private bool _isLerping;

    void Start()
    {
        GameData.Instance.PlayerRobotCharacter = this;
        Gravitated = true;
        Grounded = true;
        _currentCameraRotation = 0;
        _currentRobotRotation = 0;

        _currentForward = transform.forward;
        _zRotation = 0;
    }

    private void Update()
    {
        if (CurrentPath == null)
            UpdateEntity();
        else
            _currentRobotRotation = transform.eulerAngles.y;
    }

    public override float GetXRotation()
    {
        return _cameraBone.transform.eulerAngles.x;
    }

    public override void Move(Vector3 direction, Vector3 eulerRotate)
    {
        if (_isLerping)
            return;

        _currentCameraRotation += direction.x;

        float difference = (_currentCameraRotation - _currentRobotRotation);


        if (CurrentPath == null)
            _currentRobotRotation += difference * Time.deltaTime * 4;
        else
            _currentRobotRotation = transform.eulerAngles.y;

        transform.eulerAngles = new Vector3(0, _currentRobotRotation, 0);
        _cameraBone.localEulerAngles = new Vector3(0,180,-difference + 180);

        RaycastHit hit1;
        RaycastHit hit2;

        RaycastHit hit3;
        RaycastHit hit4;

        if( Grounded &&
            Physics.Raycast(transform.position, -transform.up - transform.forward * 0.6f, out hit1, 1) &&
            Physics.Raycast(transform.position, -transform.up + transform.forward*0.6f, out hit2, 1) &&
            Physics.Raycast(transform.position, -transform.up - transform.right * 0.3f, out hit3, 1) &&
            Physics.Raycast(transform.position, -transform.up + transform.right * 0.3f, out hit4, 1))
        {

            _currentForward += ((hit2.point - hit1.point).normalized-_currentForward)*Time.deltaTime*8;
            _currentForward.Normalize();

            float horizontal = Mathf.Sqrt(Mathf.Pow(hit3.point.x - hit4.point.x, 2) + Mathf.Pow(hit3.point.y - hit4.point.y, 2));

            _zRotation = (Mathf.Atan2(hit4.point.z - hit3.point.z, horizontal)-_zRotation)*Time.deltaTime*8;

            _robotMesh.forward = _currentForward;
            _robotMesh.eulerAngles = new Vector3(_robotMesh.eulerAngles.x, _robotMesh.eulerAngles.y, _zRotation);
        }

        base.Move(new Vector3(0, 0, direction.z), CurrentPath != null? Vector3.zero : eulerRotate);
    }

    public void PlaceDown(Transform placedFrom)
    {
        RaycastHit hit;

        if (Physics.Raycast(placedFrom.position, Vector3.down, out hit, 2, ~(1 << 6)))
        {

            StartCoroutine(AnimateLerp(placedFrom.position, hit.point, placedFrom.eulerAngles.y));
        }
    }

    private IEnumerator AnimateLerp(Vector3 startPoint, Vector3 endPoint, float angle)
    {
        _isLerping = true;
        transform.eulerAngles = new Vector3(0, angle, 0);
        _currentCameraRotation = angle;
        _currentRobotRotation = angle;

        float i = 0;

        while(i < 1)
        {
            i += Time.fixedDeltaTime * 2;

            transform.position = Vector3.Lerp(startPoint, endPoint, i);

            yield return new WaitForFixedUpdate();
        }

        _isLerping = false;
    }

    public bool CanBePickedUp()
    {
        return CurrentPath == null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public bool Gravitated { get; set; }
    public bool Grounded { get; protected set; }
    public float VerticalMovement { get; private set; }

    public EntityPath CurrentPath;
    public EntityPath CurrentToggleablePath;

    protected Quaternion _previousPathDirection;

    [SerializeField] private float _speed = 1;
    [SerializeField] private float _runFactor = 2;
    [SerializeField] private float _jumpFactor = 0.1f;

    protected bool _running = false;

    [SerializeField] protected CharacterController CC;

    [SerializeField] protected float _radius = 0.5f;

    public virtual void Move(Vector3 direction, Vector3 eulerRotate)
    {
        if (CurrentPath != null)
            FollowPath(direction);
        else if (Gravitated &&
            !(Grounded && direction.magnitude < 0.01f ))
            Walk(direction);

    }

    private void FollowPath(Vector3 direction)
    {
        Vector3 targetPosition = CurrentPath.GetPosition(this, direction);
        Vector3 difference = targetPosition - transform.position;

        if (difference.magnitude > 0.05)
            transform.position += difference.normalized * _speed * Time.deltaTime;
        else
            transform.position = targetPosition;
    }

    public void ClingToPath()
    {
        if (CurrentToggleablePath != null)
        {
            if (CurrentPath == null)
                CurrentToggleablePath.EnterPath(this);
            else
                CurrentToggleablePath.ExitPath(this);
        }
    }

    public void Jump()
    {
        CC.Move(new Vector3(0, _jumpFactor, 0));
        VerticalMovement = _jumpFactor;
        Grounded = false;
    }

    private void Walk(Vector3 direction)
    {
        float speed = _running ? _speed * _runFactor : _speed;

        Vector3 movement3D = transform.right * direction.x + transform.forward * direction.z;
        Vector3 movement = new Vector3(movement3D.x, 0, movement3D.z).normalized * Time.deltaTime * speed;

        Ray nextStepRay = new Ray(transform.position + movement, Vector3.down);
        RaycastHit hit;
        float vertical = 0;
        if (Grounded && Physics.Raycast(nextStepRay, out hit, 2))
        {
            vertical = hit.point.y - transform.position.y;
            transform.parent = hit.transform;
        }
        else if (!Grounded)
        {
            vertical = VerticalMovement;

            movement += WallProximityMagnetism(false) * Time.deltaTime;

        }

        movement.y = vertical;

        if (movement.magnitude > 0.01f) { CC.Move(movement);}
    }

    //private void Swim()

    private Vector3 WallProximityMagnetism(bool vertical)
    {
        Vector3 magnetism = new Vector3();

        RaycastHit hit;

        int notMagnetic = ~(1 << 12);

        float magnetismFactor = _radius * 2;
        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, Vector3.forward, out hit, magnetismFactor, notMagnetic))
            magnetism += hit.normal / magnetismFactor;

        if (Physics.Raycast(origin, -Vector3.forward, out hit, magnetismFactor, notMagnetic))
            magnetism += hit.normal / magnetismFactor;

        if (Physics.Raycast(origin, Vector3.right, out hit, magnetismFactor, notMagnetic))
            magnetism += hit.normal / magnetismFactor;

        if (Physics.Raycast(origin, -Vector3.right, out hit, magnetismFactor, notMagnetic))
            magnetism += hit.normal / magnetismFactor;

        return magnetism;
    }

    protected virtual void UpdateEntity()
    {
        Grounded = Physics.CheckSphere(transform.position + Vector3.up * (_radius - 0.15f), _radius-0.05f, ~(1 << 6 | 1 << 12));
        if (!Grounded)
            VerticalMovement -= 0.4f * Time.deltaTime;
        else
            VerticalMovement = 0;
    }

    public virtual float GetXRotation()
    {
        return transform.eulerAngles.x;
    }

    public float GetSpeed() { return _speed; }
}

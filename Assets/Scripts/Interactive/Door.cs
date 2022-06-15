using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector3 _openOrientation = new Vector3(0,90,0);

    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private bool _opened;
    private bool _moving;

    private Quaternion _originalRotation;

    private void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = transform.rotation * Quaternion.Euler(_openOrientation);

        _opened = false;
        _moving = false;
    }

    public void ToggleDoor()
    {
        if (!_moving)
            StartCoroutine(SetDoorState(!_opened));
    }

    private IEnumerator SetDoorState(bool opened)
    {
        _opened = opened;

        _moving = true;

        float i = 0;

        Quaternion a = (_opened ? _closedRotation : _openRotation);
        Quaternion b = (!_opened ? _closedRotation : _openRotation);

        while(i < 1)
        {
            i += Time.fixedDeltaTime * 2;

            transform.rotation = Quaternion.Lerp(a, b, i);

            yield return new WaitForFixedUpdate();
        }

        _moving = false;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Quaternion a = transform.rotation;
        Quaternion b = transform.rotation * Quaternion.Euler(_openOrientation);

        Vector3 dirOpen = a * Vector3.right;
        Vector3 dirClosed = b * Vector3.right;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(transform.position, transform.position + dirOpen * 2);
        Gizmos.DrawLine(transform.position, transform.position + dirClosed * 2);

        Gizmos.color = Color.yellow;

        int segments = 5;

        float segmentSize = 1.0f / segments;

        for(int i = 0; i < segments; i++)
        {
            float n = i * segmentSize;
            Vector3 lerpedDirA = Quaternion.Lerp(a, b, n) * Vector3.right;
            Vector3 lerpedDirB = Quaternion.Lerp(a, b, n + segmentSize) * Vector3.right;

            Gizmos.DrawLine(transform.position + lerpedDirA * 2, transform.position + lerpedDirB * 2);
        }
    }
}

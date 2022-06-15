using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCarCamera : MonoBehaviour
{

    private Vector3 _movement;
    [SerializeField] private float _lookForwardFactor = 0;
    [SerializeField] private Transform _targetForward;
    [SerializeField] private float _mouseSensitivity = 300;

    // Start is called before the first frame update
    void Start()
    {
        _movement = new Vector3();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        _movement.Set(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0);
    }

    private void FixedUpdate()
    {
        transform.localRotation *= Quaternion.Euler(_movement * Time.deltaTime * _mouseSensitivity);

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetForward.transform.rotation * Quaternion.Euler(0,90,0), _lookForwardFactor);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}

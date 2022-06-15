using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private Transform _target;

    void Update()
    {
        transform.position = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampWiggle : MonoBehaviour
{
    [SerializeField] private bool _isWiggling;
    [SerializeField] private float _wigglingRange;
    [SerializeField] private float _wigglingSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(Mathf.Sin(Time.time * _wigglingSpeed)*_wigglingRange, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}

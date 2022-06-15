using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrembleSource : MonoBehaviour
{
    [SerializeField] private float intensity = 0.1f;
    [SerializeField] private float radius = 5;
    [SerializeField] private float duration = 3;

    public void TriggerTremble()
    {
        GameData.Instance.Tremble(this);
    }

    public float GetIntensity()
    {
        return intensity;
    }

    public float GetRadius()
    {
        return radius;
    }

    public float GetDuration()
    {
        return duration;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

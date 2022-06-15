using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableShadowViewer : MonoBehaviour
{
    [SerializeField] private List<Light> _toggleableLights = new List<Light>();
    [SerializeField] private float _lightDistance = 20f;

    void Update()
    {
        foreach(Light l in _toggleableLights)
        {
            float dist = (l.transform.position - transform.position).magnitude;
            if (l.shadows == LightShadows.None && dist < _lightDistance)
                l.shadows = LightShadows.Hard;
            else if (l.shadows != LightShadows.None && dist >= _lightDistance)
                l.shadows = LightShadows.None;
        }
    }
}

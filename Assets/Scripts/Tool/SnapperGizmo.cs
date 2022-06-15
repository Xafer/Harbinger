using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnapperGizmo : MonoBehaviour
{

    [System.Serializable]
    public struct SnapPoint
    {
        public Vector3 Position;
        public Vector3 Direction;
    }

    [SerializeField]
    private List<SnapPoint> _snappingPoints = new List<SnapPoint>();

    public List<SnapPoint> SnappingPoints { get { return _snappingPoints; } private set { _snappingPoints = value; } }

    private void OnDrawGizmos()
    {
        
    }
}

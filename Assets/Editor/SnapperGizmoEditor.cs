using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapperGizmo))]
public class SnapperGizmoEditor : Editor
{
    private static SnapperGizmo _selectedPointObject = null;
    private static int _selectedPointIndex = -1;

    private Color handleColor = new Color(1,1,0.2f,0.5f);
    private Color handleColorSelected = new Color(0.6f,1,1f,0.8f);

    private void OnEnable()
    {
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= CustomOnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SnapperGizmo sg = (SnapperGizmo)target;

        if (GUI.changed) { EditorUtility.SetDirty(sg); }
    }

    private void CustomOnSceneGUI(SceneView view)
    {
        SnapperGizmo sg = (SnapperGizmo)target;

        Transform transform = sg.transform;

        for(int i = 0; i < sg.SnappingPoints.Count; i++)
        {
            SnapperGizmo.SnapPoint sp = sg.SnappingPoints[i];

            Vector3 localPosition = sp.Position.z * transform.forward + sp.Position.y * transform.up + sp.Position.x * transform.right + transform.position;
            //Gizmos.DrawMesh(GizmoMesh, localPosition, Quaternion.Euler(sp.Direction + transform.eulerAngles));

            Handles.color = sg == _selectedPointObject && _selectedPointIndex == sg.SnappingPoints.IndexOf(sp)? handleColorSelected : handleColor;
            if (Handles.Button(localPosition, Quaternion.Euler(sp.Direction + transform.eulerAngles), 1, 1, Handles.ConeHandleCap))
            {
                if (_selectedPointObject == sg)
                {
                    _selectedPointObject = null;
                    _selectedPointIndex = -1;
                }
                else if (_selectedPointIndex >= 0)
                {
                    SnapTogether(sg.transform, sp, _selectedPointObject.transform, _selectedPointObject.SnappingPoints[_selectedPointIndex]);
                    _selectedPointObject = null;
                    _selectedPointIndex = -1;
                }
                else
                {
                    _selectedPointObject = sg;
                    _selectedPointIndex = i;
                }
            }
        }
    }

    private static Vector3 GetSnapWorldPos(Transform a, SnapperGizmo.SnapPoint sa)
    {
        return sa.Position.x * a.right + sa.Position.y * a.up + sa.Position.z * a.forward + a.position;
    }

    public static void SnapTogether(Transform a, SnapperGizmo.SnapPoint pa, Transform b, SnapperGizmo.SnapPoint pb)
    {
        SnapperGizmo sga = a.GetComponent<SnapperGizmo>();
        SnapperGizmo sgb = b.GetComponent<SnapperGizmo>();

        float angleDifference = (pb.Direction.y + b.eulerAngles.y) - ((pa.Direction.y + 180 + a.eulerAngles.y) % 360);
        b.eulerAngles = new Vector3(b.eulerAngles.x, b.eulerAngles.y - angleDifference, b.eulerAngles.z);

        Vector3 offset = (GetSnapWorldPos(a, pa) - GetSnapWorldPos(b, pb));
        b.transform.position += offset;

        Debug.Log("Snapped together successfully!");
    }
}
